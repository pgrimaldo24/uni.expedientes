using MediatR;
using Unir.Expedientes.Application.Common.Models.Results;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.DownloadFicheroConsolidacionRequisitoDocumentoById
{
    public class DownloadFicheroConsolidacionRequisitoDocumentoByIdQuery : IRequest<ResultFileContentDto>
    {
        public int Id { get; set; }
        public DownloadFicheroConsolidacionRequisitoDocumentoByIdQuery(int id)
        {
            Id = id;
        }
    }
}
