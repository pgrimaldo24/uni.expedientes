using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.Financiero
{
    public class DeudaClienteModel
    {
        public string IdCliente { get; set; }
        public string Nombre { get; set; }
        public string CodigoPais { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public double Saldo { get; set; }
        public double SaldoVencidoFecha { get; set; }
        public string DivisaEmpresa { get; set; }
        public List<FacturaModel> Facturas { get; set; }
    }
}
