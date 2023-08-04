using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.Financiero
{
    public class FacturaModel
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
        public List<ReciboModel> Recibos { get; set; }
    }
}
