namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class AlumnoAdvancedSearchParameters
    {
        public string FilterNombreCompleto { get; set; }
        public int ItemsPerPage { get; set; }
        public string OrderColumnName { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
    }
}
