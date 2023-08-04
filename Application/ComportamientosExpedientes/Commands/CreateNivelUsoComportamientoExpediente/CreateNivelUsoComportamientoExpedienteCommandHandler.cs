using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateNivelUsoComportamientoExpediente
{
    public class CreateNivelUsoComportamientoExpedienteCommandHandler : IRequestHandler<CreateNivelUsoComportamientoExpedienteCommand, int>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler> _localizer;

        public CreateNivelUsoComportamientoExpedienteCommandHandler(
            IExpedientesContext context,
            IMediator mediator,
            IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _mediator = mediator;
            _localizer = localizer;
        }

        public async Task<int> Handle(CreateNivelUsoComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var comportamientoExpediente = await _context.ComportamientosExpedientes
                .Include(ce => ce.RequisitosComportamientosExpedientes)
                .ThenInclude(rce => rce.RequisitoExpediente)
                .Include(ce => ce.NivelesUsoComportamientosExpedientes)
                .ThenInclude(nuce => nuce.TipoNivelUso)
                .FirstOrDefaultAsync(ea => ea.Id == request.IdComportamiento, cancellationToken);
            if (comportamientoExpediente == null)
                throw new NotFoundException(nameof(ComportamientoExpediente), request.IdComportamiento);

            var errorMessage = ValidatePropiedades(comportamientoExpediente.NivelesUsoComportamientosExpedientes, request);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new BadRequestException(_localizer[errorMessage]);

            var nivelUsoComportamiento = await _mediator.Send(
                new AddNivelUsoComportamientoExpedienteUncommitCommand(request.NivelUsoComportamientoExpediente), cancellationToken);
            nivelUsoComportamiento.ComportamientoExpediente = comportamientoExpediente;
            await _context.NivelesUsoComportamientosExpedientes.AddAsync(nivelUsoComportamiento, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return nivelUsoComportamiento.Id;
        }

        protected internal virtual string ValidatePropiedades(ICollection<NivelUsoComportamientoExpediente> nivelesUsoComportamientos,
            CreateNivelUsoComportamientoExpedienteCommand request)
        {
            var nivelUsoComportamiento = request.NivelUsoComportamientoExpediente;
            var errorMessage = request.NivelUsoComportamientoExpediente.TipoNivelUso.Id switch
            {
                TipoNivelUso.Universidad => nivelesUsoComportamientos.Any(nuc => nuc.TipoNivelUso.Id == TipoNivelUso.Universidad
                                                && nuc.IdRefUniversidad == nivelUsoComportamiento.IdRefUniversidad) 
                                            ? "Ya existe un Nivel Asociado del mismo Tipo para la misma Universidad" : null,
                TipoNivelUso.TipoEstudio => nivelesUsoComportamientos.Any(nuc => nuc.TipoNivelUso.Id == TipoNivelUso.TipoEstudio
                                                && nuc.IdRefTipoEstudio == nivelUsoComportamiento.IdRefTipoEstudio)
                                            ? "Ya existe un Nivel Asociado del mismo Tipo para el mismo Tipo de Estudio" : null,
                TipoNivelUso.Estudio => nivelesUsoComportamientos.Any(nuc => nuc.TipoNivelUso.Id == TipoNivelUso.Estudio
                                                && nuc.IdRefEstudio == nivelUsoComportamiento.IdRefEstudio)
                                            ? "Ya existe un Nivel Asociado del mismo Tipo para el mismo Estudio" : null,
                TipoNivelUso.PlanEstudio => nivelesUsoComportamientos.Any(nuc => nuc.TipoNivelUso.Id == TipoNivelUso.PlanEstudio
                                                && nuc.IdRefPlan == nivelUsoComportamiento.IdRefPlan)
                                            ? "Ya existe un Nivel Asociado del mismo Tipo para el mismo Plan" : null,
                TipoNivelUso.TipoAsignatura => nivelesUsoComportamientos.Any(nuc => nuc.TipoNivelUso.Id == TipoNivelUso.TipoAsignatura
                                                && nuc.IdRefTipoAsignatura == nivelUsoComportamiento.IdRefTipoAsignatura
                                                && nuc.IdRefUniversidad == nivelUsoComportamiento.IdRefUniversidad
                                                && nuc.IdRefTipoEstudio == nivelUsoComportamiento.IdRefTipoEstudio
                                                && nuc.IdRefEstudio == nivelUsoComportamiento.IdRefEstudio
                                                && nuc.IdRefPlan == nivelUsoComportamiento.IdRefPlan)
                                            ? "Ya existe un Nivel Asociado del mismo Tipo para el mismo Tipo de Asignatura" : null,
                TipoNivelUso.AsignaturaPlan => nivelesUsoComportamientos.Any(nuc => nuc.TipoNivelUso.Id == TipoNivelUso.AsignaturaPlan
                                                && nuc.IdRefAsignaturaPlan == nivelUsoComportamiento.IdRefAsignaturaPlan)
                                            ? "Ya existe un Nivel Asociado del mismo Tipo para la misma Asignatura Plan" : null,
                _ => throw new BadRequestException(_localizer["Debe seleccionar un Tipo de Nivel de Uso."]),
            };

            return errorMessage;
        }
    }
}
