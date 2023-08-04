using MediatR;

namespace Unir.Expedientes.Application.Anotaciones.Queries.GetAnotacionById
{
    public class GetAnotacionByIdQuery : IRequest<AnotacionDto>
    {
        public int Id { get; set; }
        public GetAnotacionByIdQuery(int id)
        {
            Id = id;
        }
    }
}
