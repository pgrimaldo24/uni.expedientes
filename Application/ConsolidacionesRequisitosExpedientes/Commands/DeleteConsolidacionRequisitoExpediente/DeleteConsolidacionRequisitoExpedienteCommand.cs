using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.DeleteConsolidacionRequisitoExpediente
{
    public class DeleteConsolidacionRequisitoExpedienteCommand : IRequest
    {
        public int Id { get; set; }
        public DeleteConsolidacionRequisitoExpedienteCommand(int id)
        {
            Id = id;
        }
    }
}
