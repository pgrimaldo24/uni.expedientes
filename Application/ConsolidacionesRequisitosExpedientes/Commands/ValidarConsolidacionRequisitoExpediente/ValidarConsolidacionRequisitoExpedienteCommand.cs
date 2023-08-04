using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.ValidarConsolidacionRequisitoExpediente
{
    public class ValidarConsolidacionRequisitoExpedienteCommand : IRequest
    {
        public int Id { get; set; }
        public int? IdCausaEstadoRequisito { get; set; }
    }
}
