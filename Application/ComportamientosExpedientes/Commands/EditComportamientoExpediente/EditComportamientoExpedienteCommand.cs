using MediatR;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditComportamientoExpediente
{
    public class EditComportamientoExpedienteCommand : IRequest
    {
        public int IdComportamiento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EstaVigente { get; set; }
    }
}
