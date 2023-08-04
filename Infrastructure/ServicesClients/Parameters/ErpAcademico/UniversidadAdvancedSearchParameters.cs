namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class UniversidadAdvancedSearchParameters
    {
        public string FilterNombre { get; set; }
        public string OrderColumnName { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
