using MediatR;
using System;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificadoMasivo
{
    public class GenerarSolicitudTituloCertificadoMasivoCommand : ApplyQueryDto, IRequest
    {
        public int TipoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaPago { get; set; }
    }
}
