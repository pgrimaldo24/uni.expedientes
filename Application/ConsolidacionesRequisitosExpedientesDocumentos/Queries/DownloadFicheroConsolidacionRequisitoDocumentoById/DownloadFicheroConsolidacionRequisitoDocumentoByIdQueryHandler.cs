using MediatR;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Results;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Files;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.DownloadFicheroConsolidacionRequisitoDocumentoById
{
    public class DownloadFicheroConsolidacionRequisitoDocumentoByIdQueryHandler : IRequestHandler<DownloadFicheroConsolidacionRequisitoDocumentoByIdQuery, ResultFileContentDto>
    {
        private static readonly string RouteFile = $"{nameof(ConsolidacionRequisitoExpedienteDocumento)}/{nameof(ConsolidacionRequisitoExpedienteDocumento.Fichero)}/{{0}}{{1}}";

        private readonly IExpedientesContext _context;
        private readonly IFileManager _fileManager;

        public DownloadFicheroConsolidacionRequisitoDocumentoByIdQueryHandler
            (IExpedientesContext context, 
            IFileManager fileManager)
        {
            _context = context;
            _fileManager = fileManager;
        }

        public async Task<ResultFileContentDto> Handle(DownloadFicheroConsolidacionRequisitoDocumentoByIdQuery request, CancellationToken cancellationToken)
        {
            var consolidacionDocumento = await _context.ConsolidacionesRequisitosExpedientesDocumentos
                .FirstOrDefaultAsync(cred => cred.Id == request.Id, cancellationToken);
            if (consolidacionDocumento == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpedienteDocumento), request.Id);

            const string separarConcatenacion = " (";
            var nombreArchivo = consolidacionDocumento.Fichero.Split(separarConcatenacion);
            var relativePath = string.Format(RouteFile, consolidacionDocumento.Id, Path.GetExtension(nombreArchivo.First()));
            var contentFile = await _fileManager.ReadFileContentAsync(relativePath);
            return new ResultFileContentDto
            {
                Content = contentFile,
                FileName = nombreArchivo.First()
            };
        }
    }
}
