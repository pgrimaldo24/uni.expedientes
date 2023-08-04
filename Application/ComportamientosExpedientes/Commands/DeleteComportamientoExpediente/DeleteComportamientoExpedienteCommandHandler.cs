using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientoExpediente
{
    public class DeleteComportamientoExpedienteCommandHandler : IRequestHandler<DeleteComportamientoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteComportamientoExpedienteCommandHandler> _localizer;
        public DeleteComportamientoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteComportamientoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }
        public async Task<Unit> Handle(DeleteComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var comportamiento = await _context.ComportamientosExpedientes
                .Include(c => c.RequisitosComportamientosExpedientes)
                .ThenInclude(rce => rce.RequisitoExpediente)
                .ThenInclude(r => r.ConsolidacionesRequisitosExpedientes)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (comportamiento == null)
                throw new NotFoundException(nameof(ComportamientoExpediente), request.Id);

            if (comportamiento.RequisitosComportamientosExpedientes.Any(rce => 
                rce.RequisitoExpediente.ConsolidacionesRequisitosExpedientes.Any()))
                throw new BadRequestException(_localizer["Alguno de los requisitos asociados tienen consolidaciones en los expedientes."]);

            _context.Remove(comportamiento);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}