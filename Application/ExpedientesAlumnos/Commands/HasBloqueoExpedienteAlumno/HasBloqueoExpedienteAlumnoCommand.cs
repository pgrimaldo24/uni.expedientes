using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.HasBloqueoExpedienteAlumno
{
    public class HasBloqueoExpedienteAlumnoCommand : IRequest<bool>
    {
        public int IdExpedienteAlumno { get; set; }

        public HasBloqueoExpedienteAlumnoCommand(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
