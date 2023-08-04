using MediatR;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Files;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.DeleteConsolidacionRequisitoDocumentosById
{
    public class DeleteConsolidacionRequisitoDocumentosByIdCommandHandler : IRequestHandler<DeleteConsolidacionRequisitoDocumentosByIdCommand>
    {
        private static readonly string RouteFile = $"{nameof(ConsolidacionRequisitoExpedienteDocumento)}/{nameof(ConsolidacionRequisitoExpedienteDocumento.Fichero)}/{{0}}{{1}}";

        private readonly IExpedientesContext _context;
        private readonly IFileManager _fileManager;

        public DeleteConsolidacionRequisitoDocumentosByIdCommandHandler(
            IExpedientesContext context,
            IFileManager fileManager)
        {
            _context = context;
            _fileManager = fileManager;
        }

        public async Task<Unit> Handle(DeleteConsolidacionRequisitoDocumentosByIdCommand request, CancellationToken cancellationToken)
        {
            var consolidacionDocumento = await _context.ConsolidacionesRequisitosExpedientesDocumentos
                .FirstOrDefaultAsync( cred => cred.Id == request.Id, cancellationToken);
            if (consolidacionDocumento == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpedienteDocumento), request.Id);

            const string separarConcatenacion = " (";
            var nombreArchivo = consolidacionDocumento.Fichero.Split(separarConcatenacion);
            var relativePath = string.Format(RouteFile, consolidacionDocumento.Id, Path.GetExtension(nombreArchivo.First()));
            _fileManager.RemoveFile(relativePath);

            _context.ConsolidacionesRequisitosExpedientesDocumentos.Remove(consolidacionDocumento);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
