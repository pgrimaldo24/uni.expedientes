using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.ValidarConsolidacionRequisitoExpediente
{
    public class ValidarConsolidacionRequisitoExpedienteCommandHandler : IRequestHandler<ValidarConsolidacionRequisitoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<ValidarConsolidacionRequisitoExpedienteCommandHandler> _localizer;

        public ValidarConsolidacionRequisitoExpedienteCommandHandler(
           IExpedientesContext context,
           IStringLocalizer<ValidarConsolidacionRequisitoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(ValidarConsolidacionRequisitoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var consolidacionRequisito = await _context.ConsolidacionesRequisitosExpedientes
                .Include(cre => cre.RequisitoExpediente)
                .ThenInclude(re => re.RequisitosExpedientesDocumentos)
                .Include(cre => cre.ConsolidacionesRequisitosExpedientesDocumentos)
                .FirstOrDefaultAsync(cre => cre.Id == request.Id);
            if (consolidacionRequisito == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpediente), request.Id);

            await ValidatePropiedades(consolidacionRequisito, request);
            AssignConsolidacionRequisitoExpediente(consolidacionRequisito, request.IdCausaEstadoRequisito);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual async Task ValidatePropiedades(ConsolidacionRequisitoExpediente consolidacionRequisito,
            ValidarConsolidacionRequisitoExpedienteCommand request)
        {
            if (!consolidacionRequisito.EsValidoParaRechazarValidar())
                throw new BadRequestException(_localizer["El estado debe estar en estado Pendiente o Rechazado"]);

            if (request.IdCausaEstadoRequisito != null
                && !await _context.CausasEstadosRequisitosConsolidadasExpedientes
                    .AnyAsync(cerce => cerce.Id == request.IdCausaEstadoRequisito))
                throw new BadRequestException(_localizer["No existe la causa del estado del requisito"]);

            var errors = consolidacionRequisito.VerificarPropiedadesParaEstadoValidado();
            if (errors.Any())
                throw new ValidationErrorsException(errors.ToArray());
        }

        protected internal virtual void AssignConsolidacionRequisitoExpediente(
            ConsolidacionRequisitoExpediente consolidacionRequisito, int? idCausaEstadoRequisito)
        {
            consolidacionRequisito.FechaCambioEstado = DateTime.UtcNow;
            consolidacionRequisito.EstadoRequisitoExpedienteId = EstadoRequisitoExpediente.Validado;
            consolidacionRequisito.CausaEstadoRequisitoConsolidadaExpedienteId = idCausaEstadoRequisito;
        }
    }
}
