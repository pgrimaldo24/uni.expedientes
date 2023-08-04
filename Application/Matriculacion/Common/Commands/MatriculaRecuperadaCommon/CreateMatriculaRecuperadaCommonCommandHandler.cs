using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon
{
    public class CreateMatriculaRecuperadaCommonCommandHandler : IRequestHandler<CreateMatriculaRecuperadaCommonCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        public CreateMatriculaRecuperadaCommonCommandHandler(IExpedientesContext context, IMediator mediator,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<Unit> Handle(CreateMatriculaRecuperadaCommonCommand request, CancellationToken cancellationToken)
        {
            var tipoSituacionEstado = TipoSituacionEstado.CancelacionBajaMatriculaNuevoIngreso;
            var message = string.Empty;
            var expedienteHaSidoCerrado = false;

            if (request.IsAmpliacion)
            {
                tipoSituacionEstado = TipoSituacionEstado.CancelacionBajaAmpliacionMatricula;
                message = "ampliación";
            }

            if (request.EsVariacion)
            {
                tipoSituacionEstado = TipoSituacionEstado.VariacionMatriculaRecuperada;
                message = "variación";
            }

            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                request.MatriculaIdIntegracion), cancellationToken);
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var matricula = alumno.MatriculaAcademicoModel;
            var asignaturasErp = await _erpAcademicoServiceClient.GetAsignaturasMatricula(matricula.Id);
            if (asignaturasErp != null && asignaturasErp.Any())
            {
                var idsAsignaturasPlanRecuperadas = asignaturasErp
                    .Where(am => request.IdsAsignaturasOfertadas.Contains(am.AsignaturaOfertada.Id))
                    .Select(a => a.AsignaturaOfertada.AsignaturaPlan.Id).ToList();
                expedienteAlumno.CambiarSituacionAsignaturas(idsAsignaturasPlanRecuperadas, SituacionAsignatura.Matriculada);

                if (request.IdsAsignaturasExcluidas is not null)
                {
                    var idsAsignaturasExcluidasPlan = asignaturasErp
                        .Where(am => request.IdsAsignaturasExcluidas.Contains(am.AsignaturaOfertada.Id))
                        .Select(a => a.AsignaturaOfertada.AsignaturaPlan.Id).ToList();
                    expedienteAlumno.CambiarSituacionAsignaturas(idsAsignaturasExcluidasPlan, SituacionAsignatura.Anulada);
                }
            }

            var existenOtrosActivos = alumnoAcademico.Matriculas.Any(x => x.Estado.EsActiva && x.Id != matricula.Id);
            var tipoSituacionRecuperacion = await _context.TiposSituacionEstado
                .FirstAsync(ts => ts.Id == tipoSituacionEstado, cancellationToken);
            if (request.EsVariacion)
            {
                var existenOtrasAltas = alumnoAcademico.Matriculas.Any(x => x.Estado.EsAlta && x.Id != matricula.Id);

                if (!existenOtrasAltas && request.IdsAsignaturasOfertadas.Any())
                {
                    expedienteAlumno.EstadoId = EstadoExpediente.Abierto;
                    expedienteAlumno.FechaFinalizacion = null;
                    expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacionRecuperacion, request.FechaHora);
                }

                if (!existenOtrosActivos && !expedienteAlumno.AsignaturasExpedientes.Any(ae => ae.SituacionAsignaturaId != SituacionAsignatura.Anulada))
                {
                    expedienteAlumno.EstadoId = EstadoExpediente.Cerrado;
                    expedienteHaSidoCerrado = true;
                    tipoSituacionRecuperacion = await _context.TiposSituacionEstado
                        .FirstAsync(ts => ts.Id == TipoSituacionEstado.VariacionMatriculaRecuperadaDejaSinAsignaturas,
                            cancellationToken);
                    expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacionRecuperacion, request.FechaHora);
                }

                expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizado,
                    $"Recuperar {message} matrícula anulada,{matricula.Id}", request.FechaHora, request.Origen, request.Mensaje);
            }
            else
            {
                if (!existenOtrosActivos)
                {
                    expedienteAlumno.EstadoId = EstadoExpediente.Abierto;
                    if (!request.IsAmpliacion) expedienteAlumno.FechaFinalizacion = null;
                    expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacionRecuperacion, request.FechaHora);

                    var idshitosAnulados = expedienteAlumno.HitosConseguidos
                        .Where(x => x.TipoConseguidoId == TipoHitoConseguido.Anulado)
                        .Select(hc => hc.Id).ToList();
                    if (idshitosAnulados.Any())
                    {
                        var hitosEliminar = _context.HitosConseguidos.Where(hc => idshitosAnulados.Contains(hc.Id)).ToList();
                        _context.HitosConseguidos.RemoveRange(hitosEliminar);
                    }

                    expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizado,
                        $"Recuperar {message} matrícula anulada,{matricula.Id}", request.FechaHora, request.Origen, request.Mensaje);
                }
            }

            if (!expedienteHaSidoCerrado)
            {
                await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);
            }
            return Unit.Value;
        }
    }
}
