using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.CreateConsolidacionRequisitoExpediente
{
    public class CreateConsolidacionRequisitoExpedienteCommand : IRequest<int>
    {
        public int IdExpedienteAlumno { get; set; }
        public int IdRequisitoExpediente { get; set; }
    }
}
