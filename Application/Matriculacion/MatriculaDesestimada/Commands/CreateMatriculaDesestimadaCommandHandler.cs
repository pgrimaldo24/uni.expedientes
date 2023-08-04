using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaDesestimada.Commands
{
    public class CreateMatriculaDesestimadaCommandHandler : IRequestHandler<CreateMatriculaDesestimadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<CreateMatriculaDesestimadaCommandHandler> _localizer;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        public CreateMatriculaDesestimadaCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<CreateMatriculaDesestimadaCommandHandler> localizer,
            IMediator mediator,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<Unit> Handle(CreateMatriculaDesestimadaCommand request, CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                request.MatriculaIdIntegracion), cancellationToken);
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var matriculaActual = alumno.MatriculaAcademicoModel;

            var ultimoTipoSituacionExpediente = expedienteAlumno.TiposSituacionEstadoExpedientes.OrderByDescending(ts => ts.FechaInicio).FirstOrDefault(ts => ts.FechaFin == null);
            if (ultimoTipoSituacionExpediente != null)
                ultimoTipoSituacionExpediente.FechaFin = request.FechaHora;

            var tipoSituacionDesestimada = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == TipoSituacionEstado.MatriculaDesestimada, cancellationToken);
            var hitoDesestimado = await _context.TiposHitoConseguidos.FirstAsync(hc => hc.Id == TipoHitoConseguido.Desestimado);

            var tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteActualizado;
            var existenOtrosActivos = alumnoAcademico.Matriculas.Any(x => x.Estado.EsActiva && x.Id != matriculaActual.Id);
            if (!existenOtrosActivos)
            {
                tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteCerrado;
                expedienteAlumno.EstadoId = EstadoExpediente.Cerrado;
                expedienteAlumno.FechaFinalizacion = request.FechaHora;
                expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacionDesestimada, request.FechaHora);
                expedienteAlumno.AddHitosConseguidos(hitoDesestimado, request.FechaHora);
            }
            else
                await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);

            if (request.IdCausa.HasValue)
            {
                var causaBaja = await _erpAcademicoServiceClient.GetCausaBajaMatricula(request.IdCausa.Value);
                var nombre = (causaBaja != null) ? causaBaja.Nombre : "Sin nombre";
                    expedienteAlumno.AddSeguimientoNoUser(tipoSeguimientoExpediente, $"Matrícula desestimada por causa: {nombre}," +
                        $"{matriculaActual.Id}", request.FechaHora, request.Origen, request.Mensaje);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
