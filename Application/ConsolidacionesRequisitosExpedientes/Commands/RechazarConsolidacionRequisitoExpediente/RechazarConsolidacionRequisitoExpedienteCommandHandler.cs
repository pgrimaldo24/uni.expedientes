using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.RechazarConsolidacionRequisitoExpediente
{
    public class RechazarConsolidacionRequisitoExpedienteCommandHandler : IRequestHandler<RechazarConsolidacionRequisitoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<RechazarConsolidacionRequisitoExpedienteCommandHandler> _localizer;

        public RechazarConsolidacionRequisitoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<RechazarConsolidacionRequisitoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(RechazarConsolidacionRequisitoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var consolidacionRequisito = await _context.ConsolidacionesRequisitosExpedientes
                .FirstOrDefaultAsync(cre => cre.Id == request.Id);
            if (consolidacionRequisito == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpediente), request.Id);

            await ValidatePropiedades(consolidacionRequisito, request);
            AssignConsolidacionRequisitoExpediente(consolidacionRequisito, request.IdCausaEstadoRequisito);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual async Task ValidatePropiedades(ConsolidacionRequisitoExpediente consolidacionRequisito,
            RechazarConsolidacionRequisitoExpedienteCommand request)
        {
            if (!consolidacionRequisito.EsValidoParaRechazarValidar())
                throw new BadRequestException(_localizer["El estado debe estar en estado Pendiente o Rechazado"]);

            if (request.IdCausaEstadoRequisito != null
                && !await _context.CausasEstadosRequisitosConsolidadasExpedientes
                    .AnyAsync(cerce => cerce.Id == request.IdCausaEstadoRequisito))
                throw new BadRequestException(_localizer["No existe la causa del estado del requisito"]);
        }

        protected internal virtual void AssignConsolidacionRequisitoExpediente(
            ConsolidacionRequisitoExpediente consolidacionRequisito, int? idCausaEstadoRequisito)
        {
            consolidacionRequisito.FechaCambioEstado = DateTime.UtcNow;
            consolidacionRequisito.EstadoRequisitoExpedienteId = EstadoRequisitoExpediente.Rechazado;
            consolidacionRequisito.CausaEstadoRequisitoConsolidadaExpedienteId = idCausaEstadoRequisito;
        }
    }
}
