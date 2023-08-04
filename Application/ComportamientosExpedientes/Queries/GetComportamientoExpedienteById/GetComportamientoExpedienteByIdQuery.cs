using MediatR;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById
{
    public class GetComportamientoExpedienteByIdQuery : IRequest<ComportamientoExpedienteDto>
    {
        public int Id { get; set; }
        public GetComportamientoExpedienteByIdQuery(int id)
        {
            Id = id;
        }
    }
}
