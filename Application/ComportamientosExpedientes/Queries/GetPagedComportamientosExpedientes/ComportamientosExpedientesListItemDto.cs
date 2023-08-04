using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetPagedComportamientosExpedientes
{
    public class ComportamientosExpedientesListItemDto : IMapFrom<ComportamientoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EstaVigente { get; set; }
    }
}