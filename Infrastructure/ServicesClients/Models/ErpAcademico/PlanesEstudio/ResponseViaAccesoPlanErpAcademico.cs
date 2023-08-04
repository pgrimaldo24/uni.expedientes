namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseViaAccesoPlanErpAcademico
    {
        public int Id { get; set; }
        public ResponseViaAccesoErpAcademico ViaAcceso { get; set; }
        public ResponseNodoErpAcademico Nodo { get; set; }
    }
}
