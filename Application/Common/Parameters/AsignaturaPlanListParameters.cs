using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class AsignaturaPlanListParameters
    {
        public int? IdPlan { get; set; }
        public string FilterAsignaturaDisplayName { get; set; }
        public int? FilterIdUniversidad { get; set; }
        public int? FilterIdTipoEstudio { get; set; }
        public int? FilterIdEstudio { get; set; }
        public List<int> FilterIdsPlanes { get; set; }
        public int? FilterIdTipoAsignatura { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        public bool NoPaged { get; set; }
        public string Search { get; set; }
    }
}
