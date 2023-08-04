using MediatR;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditRequisitoComportamientoExpediente
{
    public class EditRequisitoComportamientoExpedienteCommand : IRequest
    {
        public int IdRequisitoComportamiento { get; set; }
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
