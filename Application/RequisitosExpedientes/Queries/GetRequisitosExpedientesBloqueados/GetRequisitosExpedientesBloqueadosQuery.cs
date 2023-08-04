using MediatR;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetRequisitosExpedientesBloqueados
{
    public class GetRequisitosExpedientesBloqueadosQuery : QueryParameters, IRequest<RequisitosExpedientesListItemDto[]>
    {
        public string FilterNombre { get; set; }
        public string FilterIdRefUniversidad { get; set; }
    }
}
