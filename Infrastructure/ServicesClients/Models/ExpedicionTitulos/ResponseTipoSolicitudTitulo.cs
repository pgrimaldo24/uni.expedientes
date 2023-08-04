namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedicionTitulos
{
    public class ResponseTipoSolicitudTitulo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string IdRefUniversidad { get; set; }
        public string RefCodigoTipoSolicitud { get; set; }
        public bool ConFechaPago { get; set; }
    }
}
