using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetTiposRequisitosExpedientes
{
    public class TipoRequisitoExpedienteListItemDto : IMapFrom<TipoRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
