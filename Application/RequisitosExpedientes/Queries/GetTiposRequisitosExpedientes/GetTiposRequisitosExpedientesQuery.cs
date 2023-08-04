using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetTiposRequisitosExpedientes
{
    public class GetTiposRequisitosExpedientesQuery : QueryParameters, IRequest<TipoRequisitoExpedienteListItemDto[]>
    {
        public string FilterNombre { get; set; }
    }
}
