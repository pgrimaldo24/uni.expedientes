using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ConsolidarRequisitosComportamientosExpediente
{
    public class ConsolidarRequisitosComportamientosExpedienteCommandHandler : IRequestHandler<ConsolidarRequisitosComportamientosExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public ConsolidarRequisitosComportamientosExpedienteCommandHandler(
            IExpedientesContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ConsolidarRequisitosComportamientosExpedienteCommand request, 
            CancellationToken cancellationToken)
        {
            var expedienteAlumno = await _context.ExpedientesAlumno
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(
                expedienteAlumno), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
