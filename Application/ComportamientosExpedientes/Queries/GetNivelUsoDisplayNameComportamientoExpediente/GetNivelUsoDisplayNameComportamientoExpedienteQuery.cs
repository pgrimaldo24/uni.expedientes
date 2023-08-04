using MediatR;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetNivelUsoDisplayNameComportamientoExpediente
{
    public class GetNivelUsoDisplayNameComportamientoExpedienteQuery : IRequest<string>
    {
        public NivelUsoComportamientoExpedienteDto NivelUsoComportamientoExpediente { get; set; }
        public GetNivelUsoDisplayNameComportamientoExpedienteQuery(NivelUsoComportamientoExpedienteDto nivelUsoComportamientoExpediente)
        {
            NivelUsoComportamientoExpediente = nivelUsoComportamientoExpediente;
        }
    }
}
