using System;
using System.Collections.Generic;

namespace Infrastructure.ServicesClients.Models.Financiero
{
    public class ResponseFacturaModel
    {
        public string NumeroPedido { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string FechaRegistro { get; set; }
        public DateTime FechaRegistroIso { get; set; }
        public string IdEntidadOperacion { get; set; }
        public double Importe { get; set; }
        public double ImportePendiente { get; set; }
        public string TerminoPago { get; set; }
        public string MetodoPago { get; set; }
        public List<ResponseReciboModel> Recibos { get; set; }
    }
}
