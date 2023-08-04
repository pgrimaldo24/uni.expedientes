using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditRequisitoComportamientoExpediente
{
    public class EditRequisitoComportamientoExpedienteCommandHandler : IRequestHandler<EditRequisitoComportamientoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler> _localizer;

        public EditRequisitoComportamientoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<EditRequisitoComportamientoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(EditRequisitoComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var requisitoComportamientoExpediente =
                await _context.RequisitosComportamientosExpedientes
                    .Include(rce => rce.ComportamientoExpediente)
                    .ThenInclude(ce => ce.RequisitosComportamientosExpedientes)
                    .ThenInclude(crce => crce.RequisitoExpediente)
                    .Include(rce => rce.RequisitoExpediente)
                    .Include(rce => rce.TipoRequisitoExpediente)
                    .FirstOrDefaultAsync(ea => ea.Id == request.IdRequisitoComportamiento, cancellationToken);
            if (requisitoComportamientoExpediente == null)
                throw new NotFoundException(nameof(RequisitoComportamientoExpediente), request.IdRequisitoComportamiento);

            await ValidatePropiedades(request, requisitoComportamientoExpediente.ComportamientoExpediente);            
            await AssignRequisitoComportamientoExpediente(request, requisitoComportamientoExpediente);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual async Task ValidatePropiedades(EditRequisitoComportamientoExpedienteCommand request,
            ComportamientoExpediente comportamientoExpediente)
        {
            if (!await _context.RequisitosExpedientes.AnyAsync(r => r.Id == request.RequisitoExpediente.Id))
                throw new BadRequestException(_localizer["No existe el requisito seleccionado."]);
            if (comportamientoExpediente.RequisitosComportamientosExpedientes
                .Any(rc => rc.RequisitoExpediente.Id == request.RequisitoExpediente.Id && rc.Id != request.IdRequisitoComportamiento))
                throw new BadRequestException(_localizer["El requisito seleccionado ya se encuentra registrado."]);
        }

        protected internal virtual async Task AssignRequisitoComportamientoExpediente(
            EditRequisitoComportamientoExpedienteCommand request, RequisitoComportamientoExpediente requisitoComportamiento)
        {
            requisitoComportamiento.RequisitoExpediente =
                await _context.RequisitosExpedientes.FirstAsync(r => r.Id == request.RequisitoExpediente.Id);
            requisitoComportamiento.TipoRequisitoExpediente =
                await _context.TiposRequisitosExpedientes.FirstAsync(e => e.Id == request.TipoRequisitoExpediente.Id);
        }
    }
}
