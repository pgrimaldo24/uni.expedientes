using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.BackgroundJob.SetEstadosHitosEspecializacionesExpedientesAlumnos;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadosHitosEspecializacionesExpedientesAlumnos;

public class SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler : IRequestHandler<SetEstadosHitosEspecializacionesExpedientesAlumnosCommand, string>
{
    private readonly IStringLocalizer<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler> _localizer;
    private readonly ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService _setEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService;

    public SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler(
        IStringLocalizer<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler> localizer,
        ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService
            setEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService)
    {
        _localizer = localizer;
        _setEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService =
            setEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService;
    }

    public async Task<string> Handle(SetEstadosHitosEspecializacionesExpedientesAlumnosCommand request,
        CancellationToken cancellationToken)
    {
        if (!(request.IdUniversidad.HasValue || request.IdEstudio.HasValue || request.FechaAperturaHasta.HasValue ||
              request.FechaAperturaDesde.HasValue))
        {
            throw new BadRequestException(_localizer["Debe especificar al menos un parámetro de búsqueda."]);
        }

        if (request.FechaAperturaHasta.HasValue && request.FechaAperturaDesde.HasValue &&
            request.FechaAperturaDesde > request.FechaAperturaHasta)
        {
            throw new BadRequestException(
                _localizer["Debe especificar una fecha de apertura final mayor o igual a la inicial."]);
        }

        var parameters = new SetEstadosHitosEspecializacionesExpedientesAlumnosParameters
        {
            IdUniversidad = request.IdUniversidad,
            IdEstudio = request.IdEstudio,
            FechaAperturaDesde = request.FechaAperturaDesde,
            FechaAperturaHasta = request.FechaAperturaHasta
        };

        return await Task.FromResult(_setEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService
            .SetEstadosHitosEspecializacionesDeExpedientesAlumnos(parameters));
    }
}