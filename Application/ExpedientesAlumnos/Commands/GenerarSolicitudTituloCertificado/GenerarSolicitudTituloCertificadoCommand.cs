using MediatR;
using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado
{
    public class GenerarSolicitudTituloCertificadoCommand : IRequest
    {
        public List<int> IdsExpedientes { get; set; }
        public int TipoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaPago { get; set; }
    }
}
