using MediatR;
using System;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedientesAlumnosBackground
{
    public class RelacionarExpedientesAlumnosBackgroundCommand : IRequest<string>
    {
        public string IdRefUniversidad { get; set; }
        public string IdRefEstudio { get; set; }
        public DateTime? FechaAperturaDesde { get; set; }
        public DateTime? FechaAperturaHasta { get; set; }
    }
}
