using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class PlanEstudioAdvancedSearchParameters
    {
        public string FilterCodigoNombre { get; set; }
        public List<int> FilterIdsUniversidades { get; set; }
        public SimpleItemFilterParameter FilterTipoEstudio { get; set; }
        public List<int> FilterIdsEstudios { get; set; }
        public string OrderColumnName { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
