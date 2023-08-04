using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class PlanEstudioListParameters
    {
        public string FilterCodigoNombre { get; set; }
        public List<int> FilterIdsUniversidades { get; set; }
        public List<int> FilterIdsCentros { get; set; }
        public List<int> FilterIdsAreasAcademicas { get; set; }
        public int? FilterIdTipoEstudio { get; set; }
        public List<int> FilterIdsEstudios { get; set; }
        public bool ProjectBaseOnly { get; set; }
        public bool NoPaged { get; set; }
        public string Search { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
    }
}
