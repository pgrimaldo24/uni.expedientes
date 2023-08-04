using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Queries.GetAllRequisitosExpedientesDocumentosByIdRequisito
{
    public class GetAllRequisitosExpedientesDocumentosByIdRequisitoQuery : IRequest<List<RequisitoExpedienteDocumentoDto>>
    {
        public int IdRequisitoExpediente { get; set; }

        public GetAllRequisitosExpedientesDocumentosByIdRequisitoQuery(int idRequisitoExpediente)
        {
            IdRequisitoExpediente = idRequisitoExpediente;
        }
    }
}
