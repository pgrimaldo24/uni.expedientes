using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud;
using Unir.Expedientes.Application.Common.Queries.GetRolesSecurity;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidarGenerarSolicitudTituloCertificado;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado
{
    public class GenerarSolicitudTituloCertificadoCommandHandler : IRequestHandler<GenerarSolicitudTituloCertificadoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IBusClientService _busClientService;
        private const int TotalMaximoExpedientes = 100;
        public GenerarSolicitudTituloCertificadoCommandHandler(
            IExpedientesContext context,
            IMediator mediator,
            IBusClientService busClientService)
        {
            _context = context;
            _mediator = mediator;
            _busClientService = busClientService;
        }

        public async Task<Unit> Handle(GenerarSolicitudTituloCertificadoCommand request, CancellationToken cancellationToken)
        {
            request.IdsExpedientes = request.IdsExpedientes?.Take(TotalMaximoExpedientes).ToList();
            var expedientes = await _context.ExpedientesAlumno
                .Where(e => request.IdsExpedientes.Contains(e.Id))
                .ToListAsync(cancellationToken);

            var tipoSolicitud = await _mediator.Send(new GetTipoSolicitudQuery(request.TipoSolicitud), cancellationToken);
            var rolesUsuario = await _mediator.Send(new GetRolesSecurityQuery(), cancellationToken);
            if (rolesUsuario.Any(rol => rol == AppConfiguration.KeyGestorRole) &&
                rolesUsuario.All(rol => rol != AppConfiguration.KeyAdminRole))
                request.FechaPago = null;

            await _mediator.Send(new ValidarGenerarSolicitudTituloCertificadoCommand(
                request, expedientes, tipoSolicitud, rolesUsuario), cancellationToken);

            await _busClientService.PublishAsync(new DocumentoTitulacionExpedientesSolicitados
            {
                TipoDocumentoSolicitud = tipoSolicitud.RefCodigoTipoSolicitud,
                FechaSolicitud = request.FechaSolicitud,
                FechaPagoDocumento = request.FechaPago,
                Expedientes = expedientes.Select(e => new ExpedienteSolicitudTitulacion
                {
                    AlumnoIdIntegracion = e.IdRefIntegracionAlumno,
                    PlanIdIntegracion = e.IdRefPlan
                }).ToList()
            });
            return Unit.Value;
        }
    }
}
