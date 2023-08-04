namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class ViaAccesoPlanAdvancedSearchParameters : PaginationAdvancedSearchParameters
    {
        public SimpleItemFilterParameter FilterExpedientes { get; set; }
        public SimpleItemFilterParameter FilterViaAcceso { get; set; }
        public SimpleItemFilterParameter FilterNodo { get; set; }
    }
}
