using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaAnuladaCommon
{
    public class CreateMatriculaAnuladaCommonCommandHandler : IRequestHandler<CreateMatriculaAnuladaCommonCommand>
    {
        public const int TipoBajaMatriculaTotal = 1;
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        public CreateMatriculaAnuladaCommonCommandHandler(IExpedientesContext context, IMediator mediator,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }
        public async Task<Unit> Handle(CreateMatriculaAnuladaCommonCommand request, CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion, request.MatriculaIdIntegracion), cancellationToken);
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var matricula = alumno.MatriculaAcademicoModel;
            await _mediator.Send(new EliminarConsolidacionRequisitosAnuladaCommonCommand(request.IdsAsignaturasOfertadas, alumno));

            if (matricula.AsignaturaMatriculadas.Any())
            {
                var idsAsignaturasPlan = matricula.AsignaturaMatriculadas
                    .Where(am => request.IdsAsignaturasOfertadas.Contains(am.AsignaturaOfertada.Id))
                    .Select(a => a.AsignaturaOfertada.AsignaturaPlan.Id).ToList();
                expedienteAlumno.CambiarSituacionAsignaturas(idsAsignaturasPlan, SituacionAsignatura.Anulada);
            }

            var hitoAnulado = await _context.TiposHitoConseguidos.FirstAsync(hc => hc.Id == TipoHitoConseguido.Anulado);
            var tipoSituacion = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == (request.IsAmpliacion ? TipoSituacionEstado.BajaAmpliacionMatricula :
            TipoSituacionEstado.BajaMatriculaNuevoIngreso), cancellationToken);

            var tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteActualizado;
            var existenOtrosMatriculasActivos = alumnoAcademico.Matriculas.Any(x => x.Estado.EsActiva && x.Id != matricula.Id);
            if (!existenOtrosMatriculasActivos && (request.IdTipoBaja == TipoBajaMatriculaTotal || !expedienteAlumno.AsignaturasExpedientes.Any(ae => ae.SituacionAsignaturaId != SituacionAsignatura.Anulada)))
            {
                tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteCerrado;
                expedienteAlumno.EstadoId = EstadoExpediente.Cerrado;
                expedienteAlumno.FechaFinalizacion = DateTime.UtcNow;
                expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacion, matricula.FechaCambioEstado);
                expedienteAlumno.AddHitosConseguidos(hitoAnulado, matricula.FechaCambioEstado);
            }
            else
                await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);

            var mensajeSeguimiento = $"Ampliación matrícula anulada, {matricula.Id}";

            if (!request.IsAmpliacion)
            {
                var causaBaja = await _erpAcademicoServiceClient.GetCausaBajaMatricula(request.IdCausaBaja);
                var tipoBaja = await _erpAcademicoServiceClient.GetTipoBajaMatricula(request.IdTipoBaja);
                var nombreCausaBaja = causaBaja != null ? causaBaja.Nombre : "Sin nombre";
                var nombreTipo = tipoBaja != null ? tipoBaja.Nombre : "Sin tipo";
                mensajeSeguimiento = $"Matrícula dada de baja {nombreTipo} por causa :{nombreCausaBaja},{matricula.Id}";
            }
            expedienteAlumno.AddSeguimientoNoUser(tipoSeguimientoExpediente, mensajeSeguimiento, matricula.FechaCambioEstado, request.Origen, request.Mensaje);
            return Unit.Value;
        }
    }
}
