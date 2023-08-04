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
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidarGenerarSolicitudTituloCertificado;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificadoMasivo
{
    public class GenerarSolicitudTituloCertificadoMasivoCommandHandler : IRequestHandler<GenerarSolicitudTituloCertificadoMasivoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IBusClientService _busClientService;
        private const int TotalMaximoExpedientes = 100;
        public GenerarSolicitudTituloCertificadoMasivoCommandHandler(
            IExpedientesContext context,
            IMediator mediator,
            IBusClientService busClientService)
        {
            _context = context;
            _mediator = mediator;
            _busClientService = busClientService;
        }

        public async Task<Unit> Handle(GenerarSolicitudTituloCertificadoMasivoCommand request, CancellationToken cancellationToken)
        {
            var queryable = _context.ExpedientesAlumno.AsNoTracking().AsQueryable();
            var queryFilter = await _mediator.Send(
                new GetApplyQueryExpedientesAlumnosQuery(queryable, request), cancellationToken);

            var expedientes = await queryFilter.Take(TotalMaximoExpedientes).ToListAsync(cancellationToken);
            var idsExpedientes = expedientes.Select(s => s.Id).ToList();

            var rolesUsuario = await _mediator.Send(new GetRolesSecurityQuery(), cancellationToken);
            if (rolesUsuario.Any(rol => rol == AppConfiguration.KeyGestorRole) && 
                rolesUsuario.All(rol => rol != AppConfiguration.KeyAdminRole))
                request.FechaPago = null;

            var generarSolicitud = new GenerarSolicitudTituloCertificadoCommand
            {
                TipoSolicitud = request.TipoSolicitud,
                FechaSolicitud = request.FechaSolicitud,
                FechaPago = request.FechaPago,
                IdsExpedientes = idsExpedientes
            };
            var tipoSolicitud = await _mediator.Send(new GetTipoSolicitudQuery(request.TipoSolicitud), cancellationToken);
            
            await _mediator.Send(new ValidarGenerarSolicitudTituloCertificadoCommand(
                generarSolicitud, expedientes, tipoSolicitud, rolesUsuario), cancellationToken);
            
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
