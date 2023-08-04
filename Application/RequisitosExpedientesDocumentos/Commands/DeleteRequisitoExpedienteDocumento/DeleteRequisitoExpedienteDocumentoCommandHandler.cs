using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitoExpedienteDocumento
{
    public class DeleteRequisitoExpedienteDocumentoCommandHandler : IRequestHandler<DeleteRequisitoExpedienteDocumentoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteRequisitoExpedienteDocumentoCommandHandler> _localizer;

        public DeleteRequisitoExpedienteDocumentoCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteRequisitoExpedienteDocumentoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(DeleteRequisitoExpedienteDocumentoCommand request, CancellationToken cancellationToken)
        {
            var requisitoExpedienteDocumento = await _context.RequisitosExpedientesDocumentos
                .Include(red => red.ConsolidacionesRequisitosExpedientesDocumentos)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (requisitoExpedienteDocumento == null)
                throw new NotFoundException(nameof(RequisitoExpedienteDocumento), request.Id);

            if (requisitoExpedienteDocumento.ConsolidacionesRequisitosExpedientesDocumentos.Any())
                throw new BadRequestException(_localizer["El documento tiene asociado una consolidación."]);

            _context.Remove(requisitoExpedienteDocumento);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
