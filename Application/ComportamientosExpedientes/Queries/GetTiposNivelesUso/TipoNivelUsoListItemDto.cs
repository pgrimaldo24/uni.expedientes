using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetTiposNivelesUso
{
    public class TipoNivelUsoListItemDto : IMapFrom<TipoNivelUso>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public bool EsUniversidad => Id == TipoNivelUso.Universidad;
        public bool EsTipoEstudio => Id == TipoNivelUso.TipoEstudio;
        public bool EsEstudio => Id == TipoNivelUso.Estudio;
        public bool EsPlanEstudio => Id == TipoNivelUso.PlanEstudio;
        public bool EsAsignaturaPlan => Id == TipoNivelUso.AsignaturaPlan;
        public bool EsTipoAsignatura=> Id == TipoNivelUso.TipoAsignatura;
    }
}
