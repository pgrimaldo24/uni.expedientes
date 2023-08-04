using MediatR;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateRequisitoComportamientoExpediente
{
    public class CreateRequisitoComportamientoExpedienteCommand : IRequest<int>
    {
        public int IdComportamiento { get; set; }
        public RequisitoExpedienteDto RequisitoExpediente { get; set; }
        public TipoRequisitoExpedienteDto TipoRequisitoExpediente { get; set; }
    }

    public class RequisitoExpedienteDto
    {
        public int Id { get; set; }
    }

    public class TipoRequisitoExpedienteDto
    {
        public int Id { get; set; }
    }
}
