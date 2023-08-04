using MediatR;
using System;
using System.Collections.Generic;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesAlumnosARelacionar
{
    public class GetExpedientesAlumnosARelacionarQuery : IRequest<List<ExpedienteAlumno>>
    {
        public int? IdExpedienteAlumno { get; set; }
        public string IdRefUniversidad { get; set; }
        public string IdRefEstudio { get; set; }
        public DateTime? FechaAperturaDesde { get; set; }
        public DateTime? FechaAperturaHasta { get; set; }
    }
}
