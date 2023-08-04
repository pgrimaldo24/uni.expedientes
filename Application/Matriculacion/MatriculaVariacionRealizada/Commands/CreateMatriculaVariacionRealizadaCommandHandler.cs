using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRealizada.Commands
{
    public class CreateMatriculaVariacionRealizadaCommandHandler : IRequestHandler<CreateMatriculaVariacionRealizadaCommand>
    {
        public const string CausaAsignaturaReconocimiento = "ASIGNATURA_RECONOCIMIENTO",
            CausaAsignaturaSuperada = "ASIGNATURA_SUPERADA";

        private readonly IExpedientesContext _context;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IMediator _mediator;
        public CreateMatriculaVariacionRealizadaCommandHandler(IExpedientesContext context, IMediator mediator,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }
        public async Task<Unit> Handle(CreateMatriculaVariacionRealizadaCommand request, CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                    request.MatriculaIdIntegracion), cancellationToken);
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var matricula = alumno.MatriculaAcademicoModel;
            var asignaturasErp = await _erpAcademicoServiceClient.GetAsignaturasMatricula(matricula.Id);

            var situacionAsignaturaAdicional = request.CausaEnumDominio == CausaAsignaturaSuperada ? SituacionAsignatura.Adaptada :
                request.CausaEnumDominio == CausaAsignaturaReconocimiento ? SituacionAsignatura.Reconocida : SituacionAsignatura.Matriculada;
            var asignaturasAdicionar = asignaturasErp
                .Where(a => request.IdsAsignaturasOfertadasAdicionadas.Contains(a.AsignaturaOfertada.Id))
                .Select(ao=>ao.AsignaturaOfertada).ToList();
            var asignaturasNuevas = await _mediator.Send(new GetAsignaturasAsociadasQuery(expedienteAlumno.AsignaturasExpedientes, 
                asignaturasAdicionar, SituacionAsignatura.Matriculada), cancellationToken);
            if (asignaturasNuevas.Any())
            {
                await AsignarSituacionAsignaturasAdicional(situacionAsignaturaAdicional, asignaturasNuevas);
                expedienteAlumno.AsignaturasExpedientes.AddRange(asignaturasNuevas);
            }

            var idsAsignaturasPlanAdicionarExistentes = asignaturasAdicionar.Select(a => a.AsignaturaPlan.Id).ToList();
            var asignaturasExpedienteExistente = expedienteAlumno.AsignaturasExpedientes.Where(ae => idsAsignaturasPlanAdicionarExistentes
            .Contains(Convert.ToInt32(ae.IdRefAsignaturaPlan))).ToList();
            if (asignaturasExpedienteExistente.Any())
                await AsignarSituacionAsignaturasAdicional(situacionAsignaturaAdicional, asignaturasExpedienteExistente);

            if (expedienteAlumno.AsignaturasExpedientes.Any())
            {
                var idsAsignaturasPlanExcluidos = asignaturasErp.Where(a => request.IdsAsignaturasOfertadasExcluidas.Contains(a.AsignaturaOfertada.Id))
                    .Select(a => a.AsignaturaOfertada.AsignaturaPlan.Id).ToList();
                var situacionAsignaturaId = request.CausaEnumDominio == CausaAsignaturaSuperada ? SituacionAsignatura.Adaptada :
                                request.CausaEnumDominio == CausaAsignaturaReconocimiento ? SituacionAsignatura.Reconocida : SituacionAsignatura.Anulada;
                expedienteAlumno.CambiarSituacionAsignaturas(idsAsignaturasPlanExcluidos, situacionAsignaturaId);
            }

            var existenMatriculasActivas = alumnoAcademico.Matriculas.Any(x => x.Estado.EsActiva);
            if (existenMatriculasActivas)
            {
                expedienteAlumno.EstadoId = EstadoExpediente.Abierto;
                await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);
            }
            else
                expedienteAlumno.EstadoId = EstadoExpediente.Cerrado;

            var tipoSituacionVariacion = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == TipoSituacionEstado.VariacionMatricula, cancellationToken);
            expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacionVariacion, request.FechaHoraAlta);

            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizado,
                $"Variación de matrícula:{matricula.Id}", request.FechaHoraAlta, request.Origen, request.Mensaje);

            var idsRefPlanAnuladas = expedienteAlumno.AsignaturasExpedientes
                .Where(ae => ae.SituacionAsignaturaId == SituacionAsignatura.Anulada).Select(a => a.IdRefAsignaturaPlan).ToList();
            var idsAsignaturasOfertadasAnuladas = asignaturasErp
                .Where(a => idsRefPlanAnuladas.Contains(a.AsignaturaOfertada.AsignaturaPlan.Id.ToString()))
                .Select(a => a.AsignaturaOfertada.Id).ToList();

            if (idsAsignaturasOfertadasAnuladas.Any())
                await _mediator.Send(new EliminarConsolidacionRequisitosAnuladaCommonCommand(idsAsignaturasOfertadasAnuladas, alumno), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual async Task AsignarSituacionAsignaturasAdicional(int situacionAsignaturaId, List<AsignaturaExpediente> asignaturasExpediente)
        {
            foreach (var asignatura in asignaturasExpediente)
                asignatura.SituacionAsignaturaId = situacionAsignaturaId;
        }
    }
}
