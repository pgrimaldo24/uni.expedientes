using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateRequisitoComportamientoExpediente
{
    public class CreateRequisitoComportamientoExpedienteCommandHandler : IRequestHandler<CreateRequisitoComportamientoExpedienteCommand, int>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler> _localizer;

        public CreateRequisitoComportamientoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<CreateRequisitoComportamientoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<int> Handle(CreateRequisitoComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var comportamientoExpediente = await _context.ComportamientosExpedientes
                .Include(ce => ce.RequisitosComportamientosExpedientes)
                .ThenInclude(rce => rce.RequisitoExpediente)
                .FirstOrDefaultAsync(ea => ea.Id == request.IdComportamiento, cancellationToken);
            if (comportamientoExpediente == null)
                throw new NotFoundException(nameof(ComportamientoExpediente), request.IdComportamiento);

            await ValidatePropiedades(request, comportamientoExpediente);
            var requisitoComportamientoExpediente = 
                await AssignNewRequisitoComportamientoExpediente(request, comportamientoExpediente);
            await _context.RequisitosComportamientosExpedientes.AddAsync(requisitoComportamientoExpediente, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return requisitoComportamientoExpediente.Id;
        }

        protected internal virtual async Task ValidatePropiedades(CreateRequisitoComportamientoExpedienteCommand request,
            ComportamientoExpediente comportamientoExpediente)
        {
            if (!await _context.RequisitosExpedientes.AnyAsync(r => r.Id == request.RequisitoExpediente.Id))
                throw new BadRequestException(_localizer["No existe el requisito seleccionado."]);

            if (comportamientoExpediente.RequisitosComportamientosExpedientes
                .Any(rc => rc.RequisitoExpediente.Id == request.RequisitoExpediente.Id))
                throw new BadRequestException(_localizer["El requisito seleccionado ya se encuentra registrado."]);
        }

        protected internal virtual async Task<RequisitoComportamientoExpediente> AssignNewRequisitoComportamientoExpediente(
            CreateRequisitoComportamientoExpedienteCommand request, ComportamientoExpediente comportamientoExpediente)
        {
            return new RequisitoComportamientoExpediente 
            {
                ComportamientoExpediente = comportamientoExpediente,
                RequisitoExpediente = await _context.RequisitosExpedientes
                                        .FirstAsync(r => r.Id == request.RequisitoExpediente.Id),
                TipoRequisitoExpediente = await _context.TiposRequisitosExpedientes
                                        .FirstAsync(e => e.Id == request.TipoRequisitoExpediente.Id)
            };
        }
    }
}
