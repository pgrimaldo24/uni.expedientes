using MediatR;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientoExpediente
{
    public class DeleteComportamientoExpedienteCommand : IRequest
    {
        public int Id { get; set; }
        public DeleteComportamientoExpedienteCommand(int id)
        {
            Id = id;
        }
    }
}