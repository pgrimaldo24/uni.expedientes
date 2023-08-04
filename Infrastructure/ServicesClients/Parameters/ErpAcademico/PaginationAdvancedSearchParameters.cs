namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class PaginationAdvancedSearchParameters
    {
        public int ItemsPerPage { get; set; }
        public string OrderColumnName { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
        public bool NoPaged { get; set; }
    }
}
