using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditViaAccesoPlanExpedienteAlumno
{
    public class EditViaAccesoPlanExpedienteAlumnoCommand : IRequest
    {
        public int IdExpedienteAlumno { get; set; }
        public string IdRefViaAccesoPlan { get; set; }

        public EditViaAccesoPlanExpedienteAlumnoCommand(int idExpedienteAlumno, string idRefViaAccesoPlan)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
            IdRefViaAccesoPlan = idRefViaAccesoPlan;
        }
    }
}
