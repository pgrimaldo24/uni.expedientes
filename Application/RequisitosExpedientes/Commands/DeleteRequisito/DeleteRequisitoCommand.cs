using MediatR;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisito
{
    public class DeleteRequisitoCommand : IRequest
    {
        public int Id { get; set; }
        public DeleteRequisitoCommand(int id)
        {
            Id = id;
        }
    }
}