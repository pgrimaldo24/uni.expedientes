using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetCausasEstadosRequisitosConsolidadasByIdRequisito
{
    public class CausaEstadoRequisitoConsolidadaExpedienteListItemDto : IMapFrom<CausaEstadoRequisitoConsolidadaExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class EstadoRequisitoExpedienteDto : IMapFrom<EstadoRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class RequisitoExpedienteDto : IMapFrom<RequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
