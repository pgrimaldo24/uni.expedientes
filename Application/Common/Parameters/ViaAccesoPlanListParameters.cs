namespace Unir.Expedientes.Application.Common.Parameters
{
    public class ViaAccesoPlanListParameters
    {
        public int Index { get; set; }
        public int Count { get; set; }
        public bool NoPaged { get; set; }
        public int? FilterIdExpediente { get; set; }
        public string FilterIdViaAcceso { get; set; }
        public int? FilterIdNodo { get; set; }
    }
}
