using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UpdateConsolidacionRequisitoDocumentosById
{
    public class UpdateConsolidacionRequisitoDocumentosByIdCommandHandler : IRequestHandler<UpdateConsolidacionRequisitoDocumentosByIdCommand>
    {
        private readonly IExpedientesContext _context;

        public UpdateConsolidacionRequisitoDocumentosByIdCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateConsolidacionRequisitoDocumentosByIdCommand request, CancellationToken cancellationToken)
        {
            var consolidacionDocumento = await _context.ConsolidacionesRequisitosExpedientesDocumentos
                .FirstOrDefaultAsync(cred => cred.Id == request.Id, cancellationToken);
            if (consolidacionDocumento == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpedienteDocumento), request.Id);

            consolidacionDocumento.FicheroValidado = request.FicheroValidado;
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
