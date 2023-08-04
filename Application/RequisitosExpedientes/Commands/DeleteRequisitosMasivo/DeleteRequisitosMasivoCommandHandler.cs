using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisitosMasivo
{
    public class DeleteRequisitosMasivoCommandHandler : IRequestHandler<DeleteRequisitosMasivoCommand, IList<string>>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<DeleteRequisitosMasivoCommandHandler> _localizer;
        public DeleteRequisitosMasivoCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<DeleteRequisitosMasivoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<IList<string>> Handle(DeleteRequisitosMasivoCommand request, CancellationToken cancellationToken)
        {
            var requisitos = await _context.RequisitosExpedientes
                .Include(r => r.ConsolidacionesRequisitosExpedientes)
                .Include(r => r.RequisitosComportamientosExpedientes)
                .Include(r => r.CausasEstadosRequisitosConsolidadasExpediente)
                .Include(r => r.RequisitosExpedientesRequerimientosTitulos)
                .Include(r => r.RequisitosExpedientesDocumentos)
                .ThenInclude(red => red.ConsolidacionesRequisitosExpedientesDocumentos)
                .Include(r => r.RequisitosExpedientesFilesType)
                .Include(r => r.RolesRequisitosExpedientes)
                .Where(a => request.IdsRequisitos.Contains(a.Id))
                .ToListAsync(cancellationToken);
            if (requisitos == null || !requisitos.Any())
                throw new BadRequestException(_localizer["Los requisitos seleccionados no existen."]);

            var errores = new List<string>();
            foreach (var requisito in requisitos)
            {
                if (requisito.ConsolidacionesRequisitosExpedientes.Any())
                    errores.Add($"El requisito '{requisito.Nombre}' tiene consolidaciones en los expedientes.");

                if (requisito.RequisitosComportamientosExpedientes.Any())
                    errores.Add($"El requisito '{requisito.Nombre}' es usado por algún comportamiento.");
            }

            var idsAExcluir = requisitos.Where(r => 
                                    r.ConsolidacionesRequisitosExpedientes.Any() || 
                                    r.RequisitosComportamientosExpedientes.Any())
                                .Select(r => r.Id);
            requisitos.RemoveAll(r => idsAExcluir.Contains(r.Id));

            DeleteDependencias(requisitos);
            _context.RequisitosExpedientes.RemoveRange(requisitos);
            await _context.SaveChangesAsync(cancellationToken);
            return errores;
        }

        protected internal virtual void DeleteDependencias(List<RequisitoExpediente> requisitos)
        {
            foreach (var requisito in requisitos)
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
}
