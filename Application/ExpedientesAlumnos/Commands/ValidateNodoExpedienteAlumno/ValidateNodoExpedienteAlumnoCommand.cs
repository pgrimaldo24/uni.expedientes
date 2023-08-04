using MediatR;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno
{
    public class ValidateNodoExpedienteAlumnoCommand : IRequest
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }

        public ValidateNodoExpedienteAlumnoCommand(ExpedienteAlumno expedienteAlumno)
        {
            ExpedienteAlumno = expedienteAlumno;
        }
    }
}
