using System;
using System.Collections.Generic;
using MediatR;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesSinEstados
{
    public class GetExpedientesSinEstadosQuery : IRequest<List<ExpedienteAlumno>>
    {
        public int? IdExpedienteAlumno { get; set; }
        public string IdRefUniversidad { get; set; }
        public string IdRefEstudio { get; set; }
        public DateTime? FechaAperturaDesde { get; set; }
        public DateTime? FechaAperturaHasta { get; set; }
    }
}
