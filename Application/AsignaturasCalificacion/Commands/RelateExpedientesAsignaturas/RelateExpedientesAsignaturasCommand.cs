using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.RelateExpedientesAsignaturas
{
    public class RelateExpedientesAsignaturasCommand : IRequest<List<string>>
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }

        public RelateExpedientesAsignaturasCommand(ExpedienteAlumno expedienteAlumno)
        {
            ExpedienteAlumno = expedienteAlumno;
        }
    }
}
