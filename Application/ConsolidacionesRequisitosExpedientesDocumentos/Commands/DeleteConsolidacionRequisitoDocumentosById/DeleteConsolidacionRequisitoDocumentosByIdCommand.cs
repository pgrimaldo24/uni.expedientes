using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.DeleteConsolidacionRequisitoDocumentosById
{
    public class DeleteConsolidacionRequisitoDocumentosByIdCommand : IRequest
    {
        public int Id { get; set; }

        public DeleteConsolidacionRequisitoDocumentosByIdCommand(int id)
        {
            Id = id;
        }
    }
}
