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

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.CreateConsolidacionRequisitoExpediente
{
    public class CreateConsolidacionRequisitoExpedienteCommandHandler : IRequestHandler<CreateConsolidacionRequisitoExpedienteCommand, int>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CreateConsolidacionRequisitoExpedienteCommandHandler> _localizer;

        public CreateConsolidacionRequisitoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<CreateConsolidacionRequisitoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<int> Handle(CreateConsolidacionRequisitoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var expedienteAlumno = await _context.ExpedientesAlumno
                        .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            var requisitoExpediente = await _context.RequisitosExpedientes
                .FirstOrDefaultAsync(ea => ea.Id == request.IdRequisitoExpediente, cancellationToken);
            if (requisitoExpediente == null)
                throw new NotFoundException(nameof(RequisitoExpediente), request.IdRequisitoExpediente);

            if (await _context.ConsolidacionesRequisitosExpedientes.AnyAsync(cre
                    => cre.ExpedienteAlumno.Id == request.IdExpedienteAlumno
                    && cre.RequisitoExpediente.Id == request.IdRequisitoExpediente, cancellationToken))
                throw new BadRequestException(_localizer["Ya existe una consolidación con el mismo requisito y expediente"]);

            var requisitoComportamientoExpediente = await _context.RequisitosComportamientosExpedientes
                .Include(rce => rce.RequisitoExpediente)
                .Include(rce => rce.ComportamientoExpediente)
                .ThenInclude(ce => ce.NivelesUsoComportamientosExpedientes)
                .FirstOrDefaultAsync(rce => rce.RequisitoExpediente.Id == requisitoExpediente.Id
                            && rce.ComportamientoExpediente.Bloqueado
                            && rce.ComportamientoExpediente.NivelesUsoComportamientosExpedientes
                                .Any(nuce => nuce.IdRefUniversidad == expedienteAlumno.IdRefUniversidad
                                            && nuce.TipoNivelUsoId == TipoNivelUso.Universidad), cancellationToken);
            if (requisitoComportamientoExpediente == null)
                throw new BadRequestException(_localizer["No se encontró el requisito comportamiento expediente"]);

            var consolidacionRequisito = new ConsolidacionRequisitoExpediente
            {
                ExpedienteAlumnoId = request.IdExpedienteAlumno,
                RequisitoExpedienteId = request.IdRequisitoExpediente,
                FechaCambioEstado = DateTime.UtcNow,
                EstadoRequisitoExpedienteId = EstadoRequisitoExpediente.NoProcesado,
                TipoRequisitoExpedienteId = requisitoComportamientoExpediente.TipoRequisitoExpedienteId
            };
            await _context.ConsolidacionesRequisitosExpedientes.AddAsync(consolidacionRequisito, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return consolidacionRequisito.Id;
        }
    }
}
