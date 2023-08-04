using MediatR;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetAllRequisitosExpedientes
{
    public class GetAllRequisitosExpedientesQuery : QueryParameters, IRequest<RequisitosExpedientesListItemDto[]>
    {
        public string FilterNombre { get; set; }
    }
}
