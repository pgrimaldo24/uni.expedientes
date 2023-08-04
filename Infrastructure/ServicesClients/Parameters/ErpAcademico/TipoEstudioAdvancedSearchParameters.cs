namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class TipoEstudioAdvancedSearchParameters
    {
        public string FilterNivelUso { get; set; }
        public SimpleItemFilterParameter FilterUniversidad { get; set; }
        public string OrderColumnName { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
