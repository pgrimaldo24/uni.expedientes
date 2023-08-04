using MediatR;

namespace Unir.Expedientes.Application.Anotaciones.Commands.DeleteAnotacion
{
    public class DeleteAnotacionCommand : IRequest
    {
        public int Id { get; set; }

        public DeleteAnotacionCommand(int id)
        {
            Id = id;
        }
    }
}
