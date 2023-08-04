using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UpdateConsolidacionRequisitoDocumentosById
{
    public class UpdateConsolidacionRequisitoDocumentosByIdCommand : IRequest
    {
        public int Id { get; set; }
        public bool FicheroValidado { get; set; }
    }
}
