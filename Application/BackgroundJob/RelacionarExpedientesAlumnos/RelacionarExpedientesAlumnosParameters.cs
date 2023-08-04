using System;

namespace Unir.Expedientes.Application.BackgroundJob.RelacionarExpedientesAlumnos
{
    public class RelacionarExpedientesAlumnosParameters
    {
        public string IdRefUniversidad { get; set; }
        public string IdRefEstudio { get; set; }
        public DateTime? FechaAperturaDesde { get; set; }
        public DateTime? FechaAperturaHasta { get; set; }
        public string AccountId { get; set; }
    }
}
