namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Global
{
    public class ResponseCentroErpAcademico
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string DisplayName { get; set; }
        public string IdRefResponsableCentro { get; set; }
        public ResponseUniversidadErpAcademico Universidad { get; set; }
    }
}
