namespace Unir.Expedientes.Application.Common.Parameters
{
    public class ModoRequerimientoDocumentacionListParameters
    {
        public string Search { get; set; }
        public int? Index { get; set; }
        public int? Count { get; set; }
        public bool NoPaged { get; set; }
    }
}