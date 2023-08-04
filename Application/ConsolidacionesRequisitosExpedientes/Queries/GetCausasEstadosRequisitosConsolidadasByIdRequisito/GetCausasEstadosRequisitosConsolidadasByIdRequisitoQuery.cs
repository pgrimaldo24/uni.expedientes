using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetCausasEstadosRequisitosConsolidadasByIdRequisito
{
    public class GetCausasEstadosRequisitosConsolidadasByIdRequisitoQuery : QueryParameters, IRequest<CausaEstadoRequisitoConsolidadaExpedienteListItemDto[]>
    {
        public int IdRequisito { get; set; }
        public int IdEstadoConsolidacion { get; set; }            
        public string FilterNombre { get; set; }
    }
}
