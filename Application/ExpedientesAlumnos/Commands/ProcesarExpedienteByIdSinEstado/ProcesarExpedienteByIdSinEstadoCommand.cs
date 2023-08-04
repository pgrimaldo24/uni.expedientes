using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarExpedienteByIdSinEstado
{
    public class ProcesarExpedienteByIdSinEstadoCommand : IRequest
    {
        public int IdExpedienteAlumno { get; set; }
    }
}
