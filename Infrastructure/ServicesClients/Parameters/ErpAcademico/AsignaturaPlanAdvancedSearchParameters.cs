using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Parameters.ErpAcademico
{
    public class AsignaturaPlanAdvancedSearchParameters
    {
        public SimpleItemFilterParameter FilterPlan { get; set; }
        public string FilterNivelUso { get; set; }
        public SimpleItemFilterParameter FilterUniversidad { get; set; }
        public SimpleItemFilterParameter FilterTipoEstudio { get; set; }
        public SimpleItemFilterParameter FilterEstudio { get; set; }
        public SimpleItemFilterParameter FilterTipoAsignatura { get; set; }
        public List<int> FilterIdsPlanes { get; set; }
        public bool NoPaged { get; set; }
        public int ItemsPerPage { get; set; }
        public string OrderColumnName { get; set; }
        public int OrderColumnPosition { get; set; }
        public string OrderDirection { get; set; }
        public int PageIndex { get; set; }
    }
}
