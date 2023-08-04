using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTiposRelacionesExpedientes
{
    public class TipoRelacionExpedienteListItemDto : IMapFrom<TipoRelacionExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool EsLogro { get; set; }
    }
}
