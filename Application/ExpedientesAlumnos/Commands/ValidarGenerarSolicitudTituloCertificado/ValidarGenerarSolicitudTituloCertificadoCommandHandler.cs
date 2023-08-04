using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidarGenerarSolicitudTituloCertificado
{
    public class ValidarGenerarSolicitudTituloCertificadoCommandHandler : IRequestHandler<ValidarGenerarSolicitudTituloCertificadoCommand>
    {
        private readonly IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler> _localizer;
        public ValidarGenerarSolicitudTituloCertificadoCommandHandler(
            IStringLocalizer<ValidarGenerarSolicitudTituloCertificadoCommandHandler> localizer)
        {
            _localizer = localizer;
        }

        public async Task<Unit> Handle(ValidarGenerarSolicitudTituloCertificadoCommand request, CancellationToken cancellationToken)
        {
            var command = request.GenerarSolicitudTituloCertificadoCommand;
            if (!command.IdsExpedientes.Any())
                throw new BadRequestException(_localizer["Debe seleccionar expedientes"]);

            if (request.Expedientes.Select(x => x.IdRefUniversidad).Distinct().Count() > 1)
                throw new BadRequestException(_localizer["No se permiten expedientes de diferentes universidades"]);

            if (command.FechaSolicitud.Date < DateTime.UtcNow.Date)
                throw new BadRequestException(_localizer["La fecha solicitud no debe ser menor a la fecha actual"]);

            if (command.FechaPago is not null && command.FechaPago.Value.Date < DateTime.UtcNow.Date)
                throw new BadRequestException(_localizer["La fecha pago no debe ser menor a la fecha actual"]);

            if (request.RolesUsuario.All(rol => rol != AppConfiguration.KeyAdminRole)) return Unit.Value;

            if (request.TipoSolicitud.ConFechaPago && command.FechaPago is null)
                throw new BadRequestException(_localizer["Debe seleccionar fecha de pago"]);

            return await Task.FromResult(Unit.Value);
        }
    }
}
