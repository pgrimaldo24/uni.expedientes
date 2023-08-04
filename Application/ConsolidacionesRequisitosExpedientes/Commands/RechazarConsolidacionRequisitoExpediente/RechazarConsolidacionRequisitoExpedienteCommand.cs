using MediatR;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.RechazarConsolidacionRequisitoExpediente
{
    public class RechazarConsolidacionRequisitoExpedienteCommand: IRequest
    {
        public int Id { get; set; }
        public int? IdCausaEstadoRequisito { get; set; }
    }
}
