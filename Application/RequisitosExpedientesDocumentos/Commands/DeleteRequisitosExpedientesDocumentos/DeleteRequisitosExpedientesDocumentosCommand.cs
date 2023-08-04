using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitosExpedientesDocumentos
{
    public class DeleteRequisitosExpedientesDocumentosCommand : IRequest<IList<string>>
    {
        public ICollection<int> IdsDocumentos { get; set; }
    }
}
