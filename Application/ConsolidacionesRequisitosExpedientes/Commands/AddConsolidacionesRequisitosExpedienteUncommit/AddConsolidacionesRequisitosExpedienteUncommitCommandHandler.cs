using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosComportamientosExpedientes.Queries.GetRequisitosComportamientosExpedientesAConsolidar;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit
{
    public class AddConsolidacionesRequisitosExpedienteUncommitCommandHandler : 
        IRequestHandler<AddConsolidacionesRequisitosExpedienteUncommitCommand>
    {
        private readonly IMediator _mediator;
        public AddConsolidacionesRequisitosExpedienteUncommitCommandHandler(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(AddConsolidacionesRequisitosExpedienteUncommitCommand request, 
            CancellationToken cancellationToken)
        {
            var expedienteAlumno = request.ExpedienteAlumno;
            var requisitosComportamientosExpediente = await _mediator.Send(new
                GetRequisitosComportamientosExpedientesAConsolidarQuery(expedienteAlumno), cancellationToken);
            if (requisitosComportamientosExpediente == null || !requisitosComportamientosExpediente.Any())
                return Unit.Value;

            foreach (var requisitoComportamiento in requisitosComportamientosExpediente)
            {
                expedienteAlumno.AddConsolidacionRequisitoExpediente(requisitoComportamiento);
            }
            return Unit.Value;
        }
    }
}
