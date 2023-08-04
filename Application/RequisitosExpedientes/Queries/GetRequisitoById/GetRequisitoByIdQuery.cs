using MediatR;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetRequisitoById
{
    public class GetRequisitoByIdQuery : IRequest<RequisitoDto>
    {
        public int Id { get; set; }
        public GetRequisitoByIdQuery(int id)
        {
            Id = id;
        }
    }
}
