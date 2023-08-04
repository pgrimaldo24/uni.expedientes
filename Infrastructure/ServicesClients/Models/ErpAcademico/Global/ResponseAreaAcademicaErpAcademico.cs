namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Global
{
    public class ResponseAreaAcademicaErpAcademico
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string DisplayName { get; set; }
        public ResponseCentroErpAcademico Centro { get; set; }
    }
}
