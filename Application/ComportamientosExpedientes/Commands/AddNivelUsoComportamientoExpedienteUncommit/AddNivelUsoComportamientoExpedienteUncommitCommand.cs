using MediatR;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit
{
    public class AddNivelUsoComportamientoExpedienteUncommitCommand : IRequest<NivelUsoComportamientoExpediente>
    {
        public NivelUsoComportamientoExpedienteDto NivelUsoComportamientoExpediente { get; set; }
        public AddNivelUsoComportamientoExpedienteUncommitCommand(NivelUsoComportamientoExpedienteDto nivelUsoComportamientoExpediente)
        {
            NivelUsoComportamientoExpediente = nivelUsoComportamientoExpediente;
        }
    }
}
