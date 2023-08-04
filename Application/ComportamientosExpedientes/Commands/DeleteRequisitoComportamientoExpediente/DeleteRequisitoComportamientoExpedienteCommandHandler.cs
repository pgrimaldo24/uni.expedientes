using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitoComportamientoExpediente
{
    public class DeleteRequisitoComportamientoExpedienteCommandHandler : IRequestHandler<DeleteRequisitoComportamientoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteRequisitoComportamientoExpedienteCommandHandler> _localizer;
        public DeleteRequisitoComportamientoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteRequisitoComportamientoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(DeleteRequisitoComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var requisitoComportamiento = await _context.RequisitosComportamientosExpedientes
                .Include(rce => rce.ComportamientoExpediente)
                .ThenInclude(ce => ce.RequisitosComportamientosExpedientes)
                .FirstOrDefaultAsync(rce => rce.Id == request.Id, cancellationToken);
            if (requisitoComportamiento == null)
                throw new NotFoundException(nameof(RequisitoComportamientoExpediente), request.Id);

            const int requisitoMinimo = 1;
            if (requisitoComportamiento.ComportamientoExpediente.
                    RequisitosComportamientosExpedientes.Count == requisitoMinimo)
                throw new BadRequestException(_localizer["Debe existir como mínimo un Requisito."]);

            _context.Remove(requisitoComportamiento);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
