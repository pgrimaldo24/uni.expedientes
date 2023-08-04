using System;
using System.Collections.Generic;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class DocumentoTitulacionExpedientesSolicitados : IMessage
    {
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaPagoDocumento { get; set; }
        public string TipoDocumentoSolicitud { get; set; }
        public List<ExpedienteSolicitudTitulacion> Expedientes { get; set; }
    }

    public class ExpedienteSolicitudTitulacion
    {
        public string AlumnoIdIntegracion { get; set; }
        public string PlanIdIntegracion { get; set; }
    }
}
