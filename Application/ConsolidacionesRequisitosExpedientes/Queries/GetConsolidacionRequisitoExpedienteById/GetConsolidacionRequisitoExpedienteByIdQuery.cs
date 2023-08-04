using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById
{
    public class GetConsolidacionRequisitoExpedienteByIdQuery : IRequest<ConsolidacionRequisitoExpedienteDto>
    {
        public int Id { get; set; }
        public GetConsolidacionRequisitoExpedienteByIdQuery(int id)
        {
            Id = id;
        }
    }
}
