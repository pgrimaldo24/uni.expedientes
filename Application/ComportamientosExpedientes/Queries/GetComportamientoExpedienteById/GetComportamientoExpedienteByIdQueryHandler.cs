using System;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById
{
    public class GetComportamientoExpedienteByIdQueryHandler : IRequestHandler<GetComportamientoExpedienteByIdQuery, ComportamientoExpedienteDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public GetComportamientoExpedienteByIdQueryHandler(
            IExpedientesContext context,
            IMapper mapper,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mapper = mapper;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }
        public async Task<ComportamientoExpedienteDto> Handle(GetComportamientoExpedienteByIdQuery request, CancellationToken cancellationToken)
        {
            var comportamiento = await _context.ComportamientosExpedientes
                .Include(c => c.RequisitosComportamientosExpedientes)
                .ThenInclude(rce => rce.RequisitoExpediente)
                .Include(c => c.RequisitosComportamientosExpedientes)
                .ThenInclude(rce => rce.TipoRequisitoExpediente)
                .Include(c => c.NivelesUsoComportamientosExpedientes)
                .ThenInclude(nuce => nuce.TipoNivelUso)
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.Bloqueado, cancellationToken);
            if (comportamiento == null)
                throw new NotFoundException(nameof(ComportamientoExpediente), request.Id);

            var result = _mapper.Map<ComportamientoExpediente, ComportamientoExpedienteDto>(comportamiento);
            foreach (var resultNivelesUsoComportamientosExpediente in result.NivelesUsoComportamientosExpedientes)
            {
                resultNivelesUsoComportamientosExpediente.DisplayName = GetDisplayNameNodoNivelVersion(resultNivelesUsoComportamientosExpediente);
            }
            await GetModoRequerimientoDocumentacion(result);
            return result;
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

        protected internal virtual async Task GetModoRequerimientoDocumentacion(
            ComportamientoExpedienteDto comportamiento)
        {
            if (comportamiento.RequisitosComportamientosExpedientes.All(rce => 
                !rce.RequisitoExpediente.IdRefModoRequerimientoDocumentacion.HasValue))
                return;

            var parameters = new ModoRequerimientoDocumentacionListParameters
            {
                NoPaged = true
            };
            var resultModosRequerimientos = await _erpAcademicoServiceClient
                .GetModosRequerimientoDocumentacion(parameters);
            if (!resultModosRequerimientos.Any()) return;

            comportamiento.RequisitosComportamientosExpedientes
                .Select(rce => rce.RequisitoExpediente)
                .Where(r => r.IdRefModoRequerimientoDocumentacion.HasValue)
                .ToList()
                .ForEach(mrd =>
                {
                    mrd.ModoRequerimientoDocumentacion = resultModosRequerimientos
                        .First(r => r.Id == mrd.IdRefModoRequerimientoDocumentacion.Value).Nombre;
                });
        }
    }
}
