using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.TiposSeguimientosExpedientes.Queries.GetTiposSeguimientosExpedientes
{
    public class TipoSeguimientoExpedienteListItemDto : IMapFrom<TipoSeguimientoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
