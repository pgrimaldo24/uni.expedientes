using MediatR;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadoHitosEspecializacionesExpedienteAlumno
{
    public class SetEstadoHitosEspecializacionesExpedienteAlumnoCommand : IRequest
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }

        public SetEstadoHitosEspecializacionesExpedienteAlumnoCommand(ExpedienteAlumno expedienteAlumno)
        {
            ExpedienteAlumno = expedienteAlumno;
        }
    }
}
