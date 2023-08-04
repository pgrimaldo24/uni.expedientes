using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class EstudioListParameters
    {
        public string FilterIdNombre { get; set; }
        public List<int> FilterIdsUniversidades { get; set; }
        public List<int> FilterIdsCentros { get; set; }
        public List<int> FilterIdsAreasAcademicas { get; set; }
        public int? FilterIdTipo { get; set; }
        public List<int> FilterIds { get; set; }
        public bool NoPaged { get; set; }
        public string Search { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
    }
}
