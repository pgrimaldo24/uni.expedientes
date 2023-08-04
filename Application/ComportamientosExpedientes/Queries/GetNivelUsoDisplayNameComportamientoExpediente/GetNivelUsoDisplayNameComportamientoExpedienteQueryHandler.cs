using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetNivelUsoDisplayNameComportamientoExpediente
{
    public class GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler : IRequestHandler<GetNivelUsoDisplayNameComportamientoExpedienteQuery, string>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler(
            IMediator mediator, 
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<string> Handle(GetNivelUsoDisplayNameComportamientoExpedienteQuery request, CancellationToken cancellationToken)
        {
            var nivelUsoComportamiento = await _mediator.Send(
                new AddNivelUsoComportamientoExpedienteUncommitCommand(request.NivelUsoComportamientoExpediente), cancellationToken);
            var nivelUsoComportamientoDto = _mapper.Map<NivelUsoComportamientoExpediente, NivelUsoComportamientoExpedienteDto>(nivelUsoComportamiento);
            var displayName = GetDisplayNameNodoNivelVersion(nivelUsoComportamientoDto);
            return displayName;
        }

        protected internal virtual string GetDisplayNameNodoNivelVersion(
            NivelUsoComportamientoExpedienteDto nivel)
        {
            var displayNameUniversidad = $"{nivel.IdRefUniversidad} - {nivel.AcronimoUniversidad}";
            var displayNameTipoEstudio = !string.IsNullOrWhiteSpace(nivel.IdRefTipoEstudio)
                ? $"| {nivel.IdRefTipoEstudio} - {nivel.NombreTipoEstudio}"
                : null;
            var displayNameEstudio = !string.IsNullOrWhiteSpace(nivel.IdRefEstudio)
                ? $"| {nivel.IdRefEstudio} - {nivel.NombreEstudio}"
                : null;
            var displayNamePlan = !string.IsNullOrWhiteSpace(nivel.IdRefPlan)
                ? $"| {nivel.IdRefPlan} - {nivel.NombrePlan}"
                : null;
            var displayNameAsignaturaPlan = !string.IsNullOrWhiteSpace(nivel.IdRefAsignaturaPlan)
                ? $"| {nivel.IdRefAsignaturaPlan}"
                : null;
            var displayNameTipoAsignatura = !string.IsNullOrWhiteSpace(nivel.IdRefTipoAsignatura)
                ? $"| {nivel.IdRefTipoAsignatura} - {nivel.NombreTipoAsignatura}"
                : null;
            var displayNameAsignatura = !string.IsNullOrWhiteSpace(nivel.NombreAsignatura)
                ? $"| {nivel.NombreAsignatura}"
                : null;
            return nivel.TipoNivelUso.Id switch
            {
                TipoNivelUso.Universidad => displayNameUniversidad,
                TipoNivelUso.TipoEstudio => displayNameUniversidad + (displayNameTipoEstudio ?? ""),
                TipoNivelUso.Estudio => displayNameUniversidad + (displayNameTipoEstudio ?? "") +
                                        (displayNameEstudio ?? ""),
                TipoNivelUso.PlanEstudio => displayNameUniversidad + (displayNameTipoEstudio ?? "") +
                                            (displayNameEstudio ?? "") +
                                            (displayNamePlan ?? ""),
                TipoNivelUso.TipoAsignatura => displayNameUniversidad + (displayNameTipoEstudio ?? "") +
                                               (displayNameEstudio ?? "") +
                                               (displayNamePlan ?? "") +
                                               (displayNameTipoAsignatura ?? ""),
                TipoNivelUso.AsignaturaPlan => displayNameUniversidad +
                                               (displayNameTipoEstudio ?? "") +
                                               (displayNameEstudio ?? "") +
                                               (displayNamePlan ?? "") +
                                               (displayNameTipoAsignatura ?? "") +
                                               (displayNameAsignaturaPlan ?? "") +
                                               (displayNameAsignatura ?? ""),
                _ => throw new NotImplementedException()
            };
        }
    }
}
