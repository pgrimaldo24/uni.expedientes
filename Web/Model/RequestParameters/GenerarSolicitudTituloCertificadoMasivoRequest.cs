using System;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificadoMasivo;
using Unir.Expedientes.WebUi.Model.CustomImpl;

namespace Unir.Expedientes.WebUi.Model.RequestParameters
{
    public class GenerarSolicitudTituloCertificadoMasivoRequest : CriteriaRequestMap<GenerarSolicitudTituloCertificadoMasivoCommand>
    {
        public int TipoSolicitud { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaPago { get; set; }
    }
}
