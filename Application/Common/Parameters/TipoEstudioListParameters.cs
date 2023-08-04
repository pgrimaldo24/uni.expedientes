namespace Unir.Expedientes.Application.Common.Parameters
{
    public class TipoEstudioListParameters
    {
        public string FilterNombre { get; set; }
        public int? FilterIdUniversidad { get; set; }
        public string Search { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
    }
}
