using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.GetConsolidacionRequisitoDocumentosByIdConsolidacion
{
    public class GetConsolidacionRequisitoDocumentosByIdConsolidacionQuery : IRequest<ConsolidacionRequisitoExpedienteDocumentoListItemDto[]>
    {
        public int IdConsolidacionRequisitoExpediente { get; set; }
        public GetConsolidacionRequisitoDocumentosByIdConsolidacionQuery(int idConsolidacionRequisitoExpediente)
        {
            IdConsolidacionRequisitoExpediente = idConsolidacionRequisitoExpediente;
        }
    }
}
