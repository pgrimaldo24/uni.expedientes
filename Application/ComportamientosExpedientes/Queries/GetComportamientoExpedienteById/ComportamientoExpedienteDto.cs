using System.Collections.Generic;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById
{
    public class ComportamientoExpedienteDto : IMapFrom<ComportamientoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EstaVigente { get; set; }
        public bool Bloqueado { get; set; }
        public IList<RequisitoComportamientoExpedienteDto> RequisitosComportamientosExpedientes { get; set; }
        public IList<NivelUsoComportamientoExpedienteDto> NivelesUsoComportamientosExpedientes { get; set; }
    }

    public class RequisitoComportamientoExpedienteDto : IMapFrom<RequisitoComportamientoExpediente>
    {
        public int Id { get; set; }
        public RequisitoExpedienteDto RequisitoExpediente { get; set; }
        public TipoRequisitoExpedienteDto TipoRequisitoExpediente { get; set; }
    }

    public class RequisitoExpedienteDto : IMapFrom<RequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool EstaVigente { get; set; }
        public bool RequeridaParaTitulo { get; set; }
        public bool RequiereDocumentacion { get; set; }
        public bool RequeridaParaPago { get; set; }
        public int? IdRefModoRequerimientoDocumentacion { get; set; }
        public string ModoRequerimientoDocumentacion { get; set; }
    }

    public class TipoRequisitoExpedienteDto : IMapFrom<TipoRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class NivelUsoComportamientoExpedienteDto : IMapFrom<NivelUsoComportamientoExpediente>
    {
        public int Id { get; set; }
        public string IdRefUniversidad { get; set; }
        public string AcronimoUniversidad { get; set; }
        public string IdRefTipoEstudio { get; set; }
        public string NombreTipoEstudio { get; set; }
        public string IdRefEstudio { get; set; }
        public string NombreEstudio { get; set; }
        public string IdRefPlan { get; set; }
        public string NombrePlan { get; set; }
        public string IdRefTipoAsignatura { get; set; }
        public string NombreTipoAsignatura { get; set; }
        public string IdRefAsignaturaPlan { get; set; }
        public string IdRefAsignatura { get; set; }
        public string NombreAsignatura { get; set; }
        public TipoNivelUsoDto TipoNivelUso { get; set; }
        public string DisplayName { get; set; }
    }

    public class TipoNivelUsoDto : IMapFrom<TipoNivelUso>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool EsUniversidad => Id == TipoNivelUso.Universidad;
        public bool EsTipoEstudio => Id == TipoNivelUso.TipoEstudio;
        public bool EsEstudio => Id == TipoNivelUso.Estudio;
        public bool EsPlanEstudio => Id == TipoNivelUso.PlanEstudio;
        public bool EsAsignaturaPlan => Id == TipoNivelUso.AsignaturaPlan;
        public bool EsTipoAsignatura => Id == TipoNivelUso.TipoAsignatura;
    }
}
