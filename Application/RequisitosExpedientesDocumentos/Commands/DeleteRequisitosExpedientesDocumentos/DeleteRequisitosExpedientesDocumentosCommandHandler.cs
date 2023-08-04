using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitosExpedientesDocumentos
{
    public class DeleteRequisitosExpedientesDocumentosCommandHandler : IRequestHandler<DeleteRequisitosExpedientesDocumentosCommand, IList<string>>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteRequisitosExpedientesDocumentosCommandHandler> _localizer;

        public DeleteRequisitosExpedientesDocumentosCommandHandler(IExpedientesContext context, IStringLocalizer<DeleteRequisitosExpedientesDocumentosCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<IList<string>> Handle(DeleteRequisitosExpedientesDocumentosCommand request, CancellationToken cancellationToken)
        {
            var documentos = await _context.RequisitosExpedientesDocumentos
                .Include(red => red.ConsolidacionesRequisitosExpedientesDocumentos)
                .Where(a => request.IdsDocumentos.Contains(a.Id))
                .ToListAsync(cancellationToken);

            if (documentos == null || !documentos.Any())
                throw new BadRequestException(_localizer["Los documentos seleccionados no existen."]);

            var errores = new List<string>();
            foreach (var documento in documentos)
            {
                if (documento.ConsolidacionesRequisitosExpedientesDocumentos.Any())
                    errores.Add($"El documento '{documento.NombreDocumento}' tiene consolidaciones asociadas en el requisito del expediente.");
            }

            var idsAExcluir = documentos.Where(r =>
                                    r.ConsolidacionesRequisitosExpedientesDocumentos.Any())
                                .Select(r => r.Id);
            documentos.RemoveAll(r => idsAExcluir.Contains(r.Id));

            _context.RequisitosExpedientesDocumentos.RemoveRange(documentos);
            await _context.SaveChangesAsync(cancellationToken);
            return errores;
        }
    }
}
