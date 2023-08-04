using MediatR;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteNivelUsoComportamientoExpediente
{
    public class DeleteNivelUsoComportamientoExpedienteCommand : IRequest
    {
        public int Id { get; set; }
        public DeleteNivelUsoComportamientoExpedienteCommand(int id)
        {
            Id = id;
        }
    }
}
