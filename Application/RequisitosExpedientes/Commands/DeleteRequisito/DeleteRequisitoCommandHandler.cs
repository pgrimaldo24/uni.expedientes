using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisito
{
    public class DeleteRequisitoCommandHandler : IRequestHandler<DeleteRequisitoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteRequisitoCommandHandler> _localizer;
        public DeleteRequisitoCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteRequisitoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(DeleteRequisitoCommand request, CancellationToken cancellationToken)
        {
            var requisito = await _context.RequisitosExpedientes
                .Include(r => r.ConsolidacionesRequisitosExpedientes)
                .Include(r => r.RequisitosComportamientosExpedientes)
                .Include(r => r.CausasEstadosRequisitosConsolidadasExpediente)
                .Include(r => r.RequisitosExpedientesRequerimientosTitulos)
                .Include(r => r.RequisitosExpedientesDocumentos)
                .ThenInclude(red => red.ConsolidacionesRequisitosExpedientesDocumentos)
                .Include(r => r.RequisitosExpedientesFilesType)
                .Include(r => r.RolesRequisitosExpedientes)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (requisito == null)
                throw new NotFoundException(nameof(RequisitoExpediente), request.Id);

            if (requisito.ConsolidacionesRequisitosExpedientes.Any())
                throw new BadRequestException(_localizer["El requisito tiene consolidaciones en los expedientes."]);

            if (requisito.RequisitosComportamientosExpedientes.Any())
                throw new BadRequestException(_localizer["El requisito es usado por algún comportamiento."]);

            DeleteDependencias(requisito);
            _context.Remove(requisito);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual void DeleteDependencias(RequisitoExpediente requisito)
        {
            var consolidacionesRequisitosExpedientesDocumentos = requisito.RequisitosExpedientesDocumentos
                .SelectMany(red => red.ConsolidacionesRequisitosExpedientesDocumentos);

            _context.ConsolidacionesRequisitosExpedientesDocumentos
                .RemoveRange(consolidacionesRequisitosExpedientesDocumentos);
            _context.CausasEstadosRequisitosConsolidadasExpedientes
                .RemoveRange(requisito.CausasEstadosRequisitosConsolidadasExpediente);
            _context.RequisitosExpedientesRequerimientosTitulos
                .RemoveRange(requisito.RequisitosExpedientesRequerimientosTitulos);
            _context.RequisitosExpedientesDocumentos
               .RemoveRange(requisito.RequisitosExpedientesDocumentos);
            _context.RequisitosExpedientesFileType
               .RemoveRange(requisito.RequisitosExpedientesFilesType);
            _context.RolesRequisitosExpedientes
               .RemoveRange(requisito.RolesRequisitosExpedientes);

            requisito.CausasEstadosRequisitosConsolidadasExpediente.Clear();
            requisito.RequisitosExpedientesRequerimientosTitulos.Clear();
            requisito.RequisitosExpedientesDocumentos.Clear();
            requisito.RequisitosExpedientesFilesType.Clear();
            requisito.RolesRequisitosExpedientes.Clear();
        }
    }
}
