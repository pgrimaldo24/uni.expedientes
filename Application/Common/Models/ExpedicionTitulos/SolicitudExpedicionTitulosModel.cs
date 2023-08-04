using System;

namespace Unir.Expedientes.Application.Common.Models.ExpedicionTitulos
{
    public class SolicitudExpedicionTitulosModel
    {
        public int Id { get; set; }
        public string CodigoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaPago { get; set; }
        public string IdRefUniversidad { get; set; }
        public UniversidadExpedicionTitulosModel Universidad { get; set; }
        public TipoExpedicionTitulosModel Tipo { get; set; }
        public DateTime FechaEstado { get; set; }
        public EstadoExpedicionTitulosModel Estado { get; set; }
    }
}
