using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ConsolidarRequisitosComportamientosExpediente
{
    public class ConsolidarRequisitosComportamientosExpedienteCommand : IRequest
    {
        public int IdExpedienteAlumno { get; set; }
        public ConsolidarRequisitosComportamientosExpedienteCommand(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
