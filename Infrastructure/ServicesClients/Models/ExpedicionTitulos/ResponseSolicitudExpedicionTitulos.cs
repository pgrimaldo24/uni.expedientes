using System;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedicionTitulos
{
    public class ResponseSolicitudExpedicionTitulos
    {
        public int Id { get; set; }
        public string CodigoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaPago { get; set; }
        public string IdRefUniversidad { get; set; }
        public ResponseUniversidadExpedicionTitulos Universidad { get; set; }
        public ResponseTipoExpedicionTitulos Tipo { get; set; }
        public DateTime FechaEstado { get; set; }
        public ResponseEstadoExpedicionTitulos Estado { get; set; }
    }
}
