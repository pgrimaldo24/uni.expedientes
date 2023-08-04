using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit
{
    public class AddNivelUsoComportamientoExpedienteUncommitCommandHandler : IRequestHandler<AddNivelUsoComportamientoExpedienteUncommitCommand, NivelUsoComportamientoExpediente>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler> _localizer;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public AddNivelUsoComportamientoExpedienteUncommitCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler> localizer,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<NivelUsoComportamientoExpediente> Handle(AddNivelUsoComportamientoExpedienteUncommitCommand request, CancellationToken cancellationToken)
        {
            var nivelUsoComportamiento = request.NivelUsoComportamientoExpediente;
            var tipoNivelUso = await _context.TiposNivelesUso.FindAsync(nivelUsoComportamiento.TipoNivelUso.Id);

            var nivelUsoComportamientoExpediente = nivelUsoComportamiento.TipoNivelUso.Id switch
            {
                TipoNivelUso.Universidad => await GetUniversidadNivelDeUso(nivelUsoComportamiento, tipoNivelUso),
                TipoNivelUso.TipoEstudio => await GetTipoEstudioNivelDeUso(nivelUsoComportamiento, tipoNivelUso),
                TipoNivelUso.Estudio => await GetEstudioNivelDeUso(nivelUsoComportamiento, tipoNivelUso),
                TipoNivelUso.PlanEstudio => await GetPlanEstudioNivelDeUso(nivelUsoComportamiento, tipoNivelUso),
                TipoNivelUso.TipoAsignatura => await GetTipoAsignaturaNivelDeUso(nivelUsoComportamiento, tipoNivelUso),
                TipoNivelUso.AsignaturaPlan => await GetAsignaturaPlanNivelDeUso(nivelUsoComportamiento, tipoNivelUso),
                _ => throw new BadRequestException(_localizer["Debe seleccionar un Tipo de Nivel de Uso."]),
            };
            return nivelUsoComportamientoExpediente;
        }

        protected internal virtual async Task<NivelUsoComportamientoExpediente> GetUniversidadNivelDeUso(
            NivelUsoComportamientoExpedienteDto nivelUsoComportamiento, TipoNivelUso tipoNivelUso)
        {
            var idUniversidad = Convert.ToInt32(nivelUsoComportamiento.IdRefUniversidad);
            var universidad = await _erpAcademicoServiceClient.GetUniversidadById(idUniversidad);
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = universidad.Acronimo,
                IdRefUniversidad = universidad.Id.ToString(),
                TipoNivelUso = tipoNivelUso
            };
            return nivelUsoComportamientoExpediente;
        }

        protected internal virtual async Task<NivelUsoComportamientoExpediente> GetTipoEstudioNivelDeUso(
            NivelUsoComportamientoExpedienteDto nivelUsoComportamiento, TipoNivelUso tipoNivelUso)
        {
            var idTipoEstudio = Convert.ToInt32(nivelUsoComportamiento.IdRefTipoEstudio);
            var tipoEstudio = await _erpAcademicoServiceClient.GetTipoEstudioById(idTipoEstudio);
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = tipoEstudio.Universidad.Acronimo,
                IdRefUniversidad = tipoEstudio.Universidad.Id.ToString(),
                IdRefTipoEstudio = tipoEstudio.Id.ToString(),
                NombreTipoEstudio = tipoEstudio.Nombre,
                TipoNivelUso = tipoNivelUso
            };
            return nivelUsoComportamientoExpediente;
        }

        protected internal virtual async Task<NivelUsoComportamientoExpediente> GetEstudioNivelDeUso(
            NivelUsoComportamientoExpedienteDto nivelUsoComportamiento, TipoNivelUso tipoNivelUso)
        {
            var idEstudio = Convert.ToInt32(nivelUsoComportamiento.IdRefEstudio);
            var estudio = await _erpAcademicoServiceClient.GetEstudioById(idEstudio);
            var tipoEstudio = await _erpAcademicoServiceClient.GetTipoEstudioById(estudio.Tipo.Id);
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = tipoEstudio.Universidad.Acronimo,
                IdRefUniversidad = tipoEstudio.Universidad.Id.ToString(),
                IdRefTipoEstudio = tipoEstudio.Id.ToString(),
                NombreTipoEstudio = tipoEstudio.Nombre,
                IdRefEstudio = estudio.Id.ToString(),
                NombreEstudio = estudio.Nombre,
                TipoNivelUso = tipoNivelUso
            };
            return nivelUsoComportamientoExpediente;
        }

        protected internal virtual async Task<NivelUsoComportamientoExpediente> GetPlanEstudioNivelDeUso(
            NivelUsoComportamientoExpedienteDto nivelUsoComportamiento, TipoNivelUso tipoNivelUso)
        {
            var idPlanEstudio = Convert.ToInt32(nivelUsoComportamiento.IdRefPlan);
            var planEstudio = await _erpAcademicoServiceClient.GetPlanEstudioById(idPlanEstudio);
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = planEstudio.Estudio.AreaAcademica.Centro.Universidad.Acronimo,
                IdRefUniversidad = planEstudio.Estudio.AreaAcademica.Centro.Universidad.Id.ToString(),
                IdRefTipoEstudio = planEstudio.Estudio.Tipo.Id.ToString(),
                NombreTipoEstudio = planEstudio.Estudio.Tipo.Nombre,
                IdRefEstudio = planEstudio.Estudio.Id.ToString(),
                NombreEstudio = planEstudio.Estudio.Nombre,
                IdRefPlan = planEstudio.Id.ToString(),
                NombrePlan = planEstudio.Nombre,
                TipoNivelUso = tipoNivelUso
            };
            return nivelUsoComportamientoExpediente;
        }

        protected internal virtual async Task<NivelUsoComportamientoExpediente> GetTipoAsignaturaNivelDeUso(
            NivelUsoComportamientoExpedienteDto nivelUsoComportamiento, TipoNivelUso tipoNivelUso)
        {
            ValidatePropiedadesRequeridasNivelTipoAsignatura(nivelUsoComportamiento);
            var idTipoAsignatura = Convert.ToInt32(nivelUsoComportamiento.IdRefTipoAsignatura);
            var tipoAsignatura = await _erpAcademicoServiceClient.GetTipoAsignaturaById(idTipoAsignatura);
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                IdRefTipoAsignatura = tipoAsignatura.Id.ToString(),
                NombreTipoAsignatura = tipoAsignatura.Nombre,
                TipoNivelUso = tipoNivelUso
            };

            var nivelUso = !string.IsNullOrWhiteSpace(nivelUsoComportamiento.IdRefPlan)
                                ? await GetPlanEstudioNivelDeUso(nivelUsoComportamiento, tipoNivelUso)
                            : !string.IsNullOrWhiteSpace(nivelUsoComportamiento.IdRefEstudio)
                                ? await GetEstudioNivelDeUso(nivelUsoComportamiento, tipoNivelUso)
                            : !string.IsNullOrWhiteSpace(nivelUsoComportamiento.IdRefTipoEstudio)
                                ? await GetTipoEstudioNivelDeUso(nivelUsoComportamiento, tipoNivelUso)
                            : null;

            if (nivelUso != null)
            {
                AssignNivelUsoTipoAsignatura(nivelUso, nivelUsoComportamientoExpediente);
                return nivelUsoComportamientoExpediente;
            }

            nivelUso = await GetUniversidadNivelDeUso(nivelUsoComportamiento, tipoNivelUso);
            nivelUsoComportamientoExpediente.AcronimoUniversidad = nivelUso.AcronimoUniversidad;
            nivelUsoComportamientoExpediente.IdRefUniversidad = nivelUso.IdRefUniversidad;
            return nivelUsoComportamientoExpediente;
        }

        protected internal virtual void ValidatePropiedadesRequeridasNivelTipoAsignatura(
            NivelUsoComportamientoExpedienteDto nivelUsoComportamiento)
        {
            if (string.IsNullOrWhiteSpace(nivelUsoComportamiento.IdRefTipoAsignatura))
                throw new BadRequestException(_localizer["Debe seleccionar un Tipo de Asignatura."]);

            if (string.IsNullOrWhiteSpace(nivelUsoComportamiento.IdRefUniversidad))
                throw new BadRequestException(_localizer["Debe seleccionar una Universidad."]);
        }

        protected internal virtual void AssignNivelUsoTipoAsignatura(NivelUsoComportamientoExpediente nivelUso,
            NivelUsoComportamientoExpediente nivelUsoComportamientoExpediente)
        {
            nivelUsoComportamientoExpediente.AcronimoUniversidad = nivelUso.AcronimoUniversidad;
            nivelUsoComportamientoExpediente.IdRefUniversidad = nivelUso.IdRefUniversidad;
            nivelUsoComportamientoExpediente.IdRefTipoEstudio = nivelUso.IdRefTipoEstudio;
            nivelUsoComportamientoExpediente.NombreTipoEstudio = nivelUso.NombreTipoEstudio;
            nivelUsoComportamientoExpediente.IdRefEstudio = nivelUso.IdRefEstudio;
            nivelUsoComportamientoExpediente.NombreEstudio = nivelUso.NombreEstudio;
            nivelUsoComportamientoExpediente.IdRefPlan = nivelUso.IdRefPlan;
            nivelUsoComportamientoExpediente.NombrePlan = nivelUso.NombrePlan;
        }

        protected internal virtual async Task<NivelUsoComportamientoExpediente> GetAsignaturaPlanNivelDeUso(
            NivelUsoComportamientoExpedienteDto nivelUsoComportamiento, TipoNivelUso tipoNivelUso)
        {
            var idAsignaturaPlan = Convert.ToInt32(nivelUsoComportamiento.IdRefAsignaturaPlan);
            var asignatura = await _erpAcademicoServiceClient.GetAsignaturaPlanById(idAsignaturaPlan);
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = asignatura.Plan.Estudio.AreaAcademica.Centro.Universidad.Acronimo,
                IdRefUniversidad = asignatura.Plan.Estudio.AreaAcademica.Centro.Universidad.Id.ToString(),
                IdRefTipoEstudio = asignatura.Plan.Estudio.Tipo.Id.ToString(),
                NombreTipoEstudio = asignatura.Plan.Estudio.Tipo.Nombre,
                IdRefEstudio = asignatura.Plan.Estudio.Id.ToString(),
                NombreEstudio = asignatura.Plan.Estudio.Nombre,
                IdRefPlan = asignatura.Plan.Id.ToString(),
                NombrePlan = asignatura.Plan.Nombre,
                IdRefTipoAsignatura = asignatura.Asignatura.TipoAsignatura.Id.ToString(),
                NombreTipoAsignatura = asignatura.Asignatura.TipoAsignatura.Nombre,
                IdRefAsignaturaPlan = asignatura.Id.ToString(),
                IdRefAsignatura = asignatura.Asignatura.Id.ToString(),
                NombreAsignatura = asignatura.Asignatura.Nombre,
                TipoNivelUso = tipoNivelUso
            };
            return nivelUsoComportamientoExpediente;
        }
    }
}
