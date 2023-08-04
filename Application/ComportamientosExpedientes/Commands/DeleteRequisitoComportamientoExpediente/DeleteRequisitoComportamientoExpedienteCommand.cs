using MediatR;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitoComportamientoExpediente
{
    public class DeleteRequisitoComportamientoExpedienteCommand : IRequest
    {
        public int Id { get; set; }
        public DeleteRequisitoComportamientoExpedienteCommand(int id)
        {
            Id = id;
        }
    }
}
