namespace Unir.Expedientes.Application.Common.Parameters
{
    public class ViaAccesoListParameters
    {
        public int Index { get; set; }
        public int Count { get; set; }
        public bool NoPaged { get; set; }
        public int? FilterIdNodo { get; set; }
        public bool ProjectViasAccesosGroupedBySuperViaAcceso { get; set; }
        public bool FilterEsVigente { get; set; }
    }
}
