namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class ViaAccesoAdvancedSearchParameters : PaginationAdvancedSearchParameters
    {
        public bool ProjectViasAccesosGroupedBySuperViaAcceso { get; set; }
        public bool? FilterEsVigente { get; set; }
        public SimpleItemFilterParameter FilterNodo { get; set; }
    }
}
