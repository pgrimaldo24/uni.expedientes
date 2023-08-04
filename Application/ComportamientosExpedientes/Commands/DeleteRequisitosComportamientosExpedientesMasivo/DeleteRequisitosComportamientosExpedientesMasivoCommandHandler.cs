using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitosComportamientosExpedientesMasivo
{
    public class DeleteRequisitosComportamientosExpedientesMasivoCommandHandler :
        IRequestHandler<DeleteRequisitosComportamientosExpedientesMasivoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteRequisitosComportamientosExpedientesMasivoCommandHandler> _localizer;
        public DeleteRequisitosComportamientosExpedientesMasivoCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteRequisitosComportamientosExpedientesMasivoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(DeleteRequisitosComportamientosExpedientesMasivoCommand request, CancellationToken cancellationToken)
        {
            var requisitosComportamientos = await _context.RequisitosComportamientosExpedientes
                .Include(rce => rce.ComportamientoExpediente)
                .ThenInclude(ce => ce.RequisitosComportamientosExpedientes)
                .Where(a => request.IdsRequisitosComportamientos.Contains(a.Id))
                .ToListAsync(cancellationToken);
            if (requisitosComportamientos == null || !requisitosComportamientos.Any())
                throw new BadRequestException(_localizer["Los requisitos comportamientos seleccionados no existen."]);

            var requisitosComportamientoExpedientes = requisitosComportamientos.First()
                .ComportamientoExpediente.RequisitosComportamientosExpedientes.ToList();
            if (requisitosComportamientoExpedientes.Count == request.IdsRequisitosComportamientos.Count)
                throw new BadRequestException(_localizer["Como mínimo debe mantener un Requisito."]);

            _context.RequisitosComportamientosExpedientes.RemoveRange(requisitosComportamientos);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
