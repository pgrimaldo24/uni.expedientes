using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.BackgroundJob.MigrarCalificacionesExpedientes;
using Unir.Expedientes.Application.Common.Queries.GetInfoSecurity;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionesExpedientesBackground
{
    public class MigrarCalificacionesExpedientesBackgroundCommandHandler : IRequestHandler<MigrarCalificacionesExpedientesBackgroundCommand, string>
    {
        private readonly IStringLocalizer<MigrarCalificacionesExpedientesBackgroundCommandHandler> _localizer;
        private readonly IMigrarCalificacionesExpedientesApplicationService _migrarCalificacionesExpedientesApplicationService;
        private readonly IMediator _mediator;
        public MigrarCalificacionesExpedientesBackgroundCommandHandler(
            IStringLocalizer<MigrarCalificacionesExpedientesBackgroundCommandHandler> localizer,
            IMigrarCalificacionesExpedientesApplicationService migrarCalificacionesExpedientesApplicationService,
            IMediator mediator)
        {
            _localizer = localizer;
            _migrarCalificacionesExpedientesApplicationService = migrarCalificacionesExpedientesApplicationService;
            _mediator = mediator;
        }

        public async Task<string> Handle(MigrarCalificacionesExpedientesBackgroundCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.IdRefUniversidad) && string.IsNullOrWhiteSpace(request.IdRefEstudio))
            {
                throw new BadRequestException(_localizer["Debe especificar al menos un parámetro de búsqueda."]);
            }

            var parameters = new MigrarCalificacionesExpedientesParameters
            {
                IdRefUniversidad = request.IdRefUniversidad,
                IdRefEstudio = request.IdRefEstudio
            };
            var identityInfo = await _mediator.Send(new GetInfoSecurityQuery(), cancellationToken);
            return await Task.FromResult(_migrarCalificacionesExpedientesApplicationService
                                            .MigrarCalificacionesExpedientes(identityInfo.IdRefCuentaSeguridad, identityInfo.DisplayName, parameters));
        }
    }
}
