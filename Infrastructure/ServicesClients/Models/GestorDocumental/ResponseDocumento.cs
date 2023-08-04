namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.GestorDocumental
{
    public class ResponseDocumento
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Error { get; set; }
        public int ElementoNegocio { get; set; }
        public int IdDocumentoAlumno { get; set; }
    }
}
