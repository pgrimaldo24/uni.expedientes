namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.GestorDocumental
{
    public class ResponseClasificacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Codificacion { get; set; }
        public string DisplayName { get; set; }
        public bool ConfirmarModificacion { get; set; }
    }
}
