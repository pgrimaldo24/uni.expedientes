using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.BackgroundJob.RelacionarExpedientesAlumnos;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedientesAlumnosBackground
{
    public class RelacionarExpedientesAlumnosBackgroundCommandHandler : IRequestHandler<RelacionarExpedientesAlumnosBackgroundCommand, string>
    {
        private readonly IStringLocalizer<RelacionarExpedientesAlumnosBackgroundCommandHandler> _localizer;
        private readonly IRelacionarExpedientesAlumnosApplicationService _relacionExpedientesAlumnosApplicationService;

        public RelacionarExpedientesAlumnosBackgroundCommandHandler(
            IStringLocalizer<RelacionarExpedientesAlumnosBackgroundCommandHandler> localizer,
            IRelacionarExpedientesAlumnosApplicationService relacionExpedientesAlumnosApplicationService)
        {
            _localizer = localizer;
            _relacionExpedientesAlumnosApplicationService = relacionExpedientesAlumnosApplicationService;
        }

        public async Task<string> Handle(RelacionarExpedientesAlumnosBackgroundCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.IdRefUniversidad) && string.IsNullOrWhiteSpace(request.IdRefEstudio) 
                && !request.FechaAperturaHasta.HasValue && !request.FechaAperturaDesde.HasValue)
            {
                throw new BadRequestException(_localizer["Debe especificar al menos un parámetro de búsqueda."]);
            }

            if (request.FechaAperturaHasta.HasValue && request.FechaAperturaDesde.HasValue &&
                request.FechaAperturaDesde > request.FechaAperturaHasta)
            {
                throw new BadRequestException(
                    _localizer["Debe especificar una fecha de apertura final mayor o igual a la inicial."]);
            }

            var parameters = new RelacionarExpedientesAlumnosParameters
            {
                IdRefUniversidad = request.IdRefUniversidad,
                IdRefEstudio = request.IdRefEstudio,
                FechaAperturaDesde = request.FechaAperturaDesde,
                FechaAperturaHasta = request.FechaAperturaHasta
            };

            return await Task.FromResult(_relacionExpedientesAlumnosApplicationService
                                            .RelacionarExpedientesAlumnos(parameters));
        }
    }
}
