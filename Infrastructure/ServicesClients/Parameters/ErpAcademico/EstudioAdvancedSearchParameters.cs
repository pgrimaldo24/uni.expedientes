using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class EstudioAdvancedSearchParameters
    {
        public string FilterIdNombre { get; set; }
        public IEnumerable<int> FilterIds { get; set; }
        public IEnumerable<int> FilterIdsUniversidades { get; set; }
        public IEnumerable<int> FilterIdsCentros { get; set; }
        public IEnumerable<int> FilterIdsAreasAcademicas { get; set; }
        public SimpleItemFilterParameter FilterTipo { get; set; }
        public bool NoPaged { get; set; }
        public string OrderColumnName { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
