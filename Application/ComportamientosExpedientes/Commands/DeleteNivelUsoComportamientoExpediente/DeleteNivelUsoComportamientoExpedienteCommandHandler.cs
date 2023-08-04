using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteNivelUsoComportamientoExpediente
{
    public class DeleteNivelUsoComportamientoExpedienteCommandHandler : 
        IRequestHandler<DeleteNivelUsoComportamientoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteNivelUsoComportamientoExpedienteCommandHandler> _localizer;
        public DeleteNivelUsoComportamientoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteNivelUsoComportamientoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(DeleteNivelUsoComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var nivelUsoComportamiento = await _context.NivelesUsoComportamientosExpedientes
                .Include(rce => rce.ComportamientoExpediente)
                .ThenInclude(ce => ce.NivelesUsoComportamientosExpedientes)
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (nivelUsoComportamiento == null)
                throw new NotFoundException(nameof(NivelUsoComportamientoExpediente), request.Id);

            const int nivelUsoMinimo = 1;
            if (nivelUsoComportamiento.ComportamientoExpediente.
                    NivelesUsoComportamientosExpedientes.Count == nivelUsoMinimo)
                throw new BadRequestException(_localizer["Debe existir como mínimo un Nivel de Uso."]);

            _context.Remove(nivelUsoComportamiento);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
