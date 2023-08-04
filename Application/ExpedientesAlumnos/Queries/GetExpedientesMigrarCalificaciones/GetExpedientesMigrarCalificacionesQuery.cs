using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesMigrarCalificaciones
{
    public class GetExpedientesMigrarCalificacionesQuery : IRequest<List<ExpedienteAlumno>>
    {
        public string IdRefUniversidad { get; set; }
        public string IdRefEstudio { get; set; }
    }
}
