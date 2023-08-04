namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class RequerimientoPlanAdvancedSearchParameters
    {
        public SimpleItemFilterParameter FilterPlan { get; set; }
        public string FilterNombre { get; set; }
        public int PageIndex { get; set; }
        public int ItemsPerPage { get; set; }
        public string OrderColumnName { get; set; }
        public string OrderDirection { get; set; }
    }
}
