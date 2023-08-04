using MediatR;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitoExpedienteDocumento
{
    public class DeleteRequisitoExpedienteDocumentoCommand : IRequest
    {
        public int Id { get; set; }

        public DeleteRequisitoExpedienteDocumentoCommand(int id)
        {
            Id = id;
        }
    }
}
