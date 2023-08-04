using MediatR;
using Microsoft.AspNetCore.Http;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UploadFileConsolidacionRequisitoDocumento
{
    public class UploadFileConsolidacionRequisitoDocumentoCommand : IRequest<int>
    {
        public int IdConsolidacionRequisito { get; set; }
        public int IdRequisitoDocumento { get; set; }
        public IFormFile File { get; set; }
    }
}
