using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.DeleteConsolidacionRequisitoExpediente
{
    public class DeleteConsolidacionRequisitoExpedienteCommandHandler : IRequestHandler<DeleteConsolidacionRequisitoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteConsolidacionRequisitoExpedienteCommandHandler> _localizer;

        public DeleteConsolidacionRequisitoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteConsolidacionRequisitoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(DeleteConsolidacionRequisitoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var consolidacionRequisito = await _context.ConsolidacionesRequisitosExpedientes
                .Include(cre => cre.RequisitoExpediente)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (consolidacionRequisito == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpediente), request.Id);

            if (!consolidacionRequisito.EsValidoParaEliminar())
                throw new BadRequestException(_localizer["La consolidación no debe estar en estado Validado."]);
            
            _context.Remove(consolidacionRequisito);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
