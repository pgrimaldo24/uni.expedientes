using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientosExpedientesMasivo
{
    public class DeleteComportamientosExpedientesMasivoCommandHandler : IRequestHandler<DeleteComportamientosExpedientesMasivoCommand, IList<string>>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteComportamientosExpedientesMasivoCommandHandler> _localizer;
        public DeleteComportamientosExpedientesMasivoCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteComportamientosExpedientesMasivoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<IList<string>> Handle(DeleteComportamientosExpedientesMasivoCommand request, CancellationToken cancellationToken)
        {
            var comportamientos = await _context.ComportamientosExpedientes
                .Include(c => c.RequisitosComportamientosExpedientes)
                .ThenInclude(rce => rce.RequisitoExpediente)
                .ThenInclude(r => r.ConsolidacionesRequisitosExpedientes)
                .Where(a => request.IdsComportamientos.Contains(a.Id))
                .ToListAsync(cancellationToken);
            if (comportamientos == null || !comportamientos.Any())
                throw new BadRequestException(_localizer["Los comportamientos seleccionados no existen."]);

            var errores = new List<string>();
            foreach (var comportamiento in comportamientos)
            {
                if (comportamiento.RequisitosComportamientosExpedientes.Any(rce =>
                    rce.RequisitoExpediente.ConsolidacionesRequisitosExpedientes.Any()))
                    errores.Add($"En el comportamiento '{comportamiento.Nombre}' algunos requisitos asociados tienen consolidaciones en los expedientes.");
            }

            var idsAExcluir = comportamientos.Where(c => 
                                c.RequisitosComportamientosExpedientes.Any(rce => 
                                    rce.RequisitoExpediente.ConsolidacionesRequisitosExpedientes.Any()))
                                .Select(r => r.Id);
            comportamientos.RemoveAll(r => idsAExcluir.Contains(r.Id));

            _context.ComportamientosExpedientes.RemoveRange(comportamientos);
            await _context.SaveChangesAsync(cancellationToken);
            return errores;
        }
    }
}
