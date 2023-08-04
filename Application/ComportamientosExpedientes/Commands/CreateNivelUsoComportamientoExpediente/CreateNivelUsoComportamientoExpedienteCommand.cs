using MediatR;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateNivelUsoComportamientoExpediente
{
    public class CreateNivelUsoComportamientoExpedienteCommand : IRequest<int>
    {
        public int IdComportamiento { get; set; }
        public NivelUsoComportamientoExpedienteDto NivelUsoComportamientoExpediente { get; set; }
    }
}
