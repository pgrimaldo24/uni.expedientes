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

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente
{
    public class CreateComportamientoExpedienteCommandHandler : IRequestHandler<CreateComportamientoExpedienteCommand, int>
    {
        private readonly IExpedientesContext _context;        
        private readonly IStringLocalizer<CreateComportamientoExpedienteCommandHandler> _localizer;
        private readonly IMediator _mediator;

        public CreateComportamientoExpedienteCommandHandler(
            IExpedientesContext context, 
            IStringLocalizer<CreateComportamientoExpedienteCommandHandler> localizer,
            IMediator mediator)
        {
            _context = context;
            _localizer = localizer;
            _mediator = mediator;
        }

        public async Task<int> Handle(CreateComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            await ValidatePropiedadesRequeridas(request);
            var idsRequisitos = request.RequisitosComportamientosExpedientes.Select(rce => rce.RequisitoExpediente.Id);
            var requisitos = await _context.RequisitosExpedientes
                                .Where(r => idsRequisitos.Contains(r.Id))
                                .ToListAsync(cancellationToken);

            var tiposRequisitos = await _context.TiposRequisitosExpedientes.ToListAsync(cancellationToken);

            var newComportamiento = AssignNewComportamientoExpediente(request, requisitos, tiposRequisitos, cancellationToken);
            await _context.ComportamientosExpedientes.AddAsync(newComportamiento, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return newComportamiento.Id;
        }

        protected internal virtual async Task ValidatePropiedadesRequeridas(CreateComportamientoExpedienteCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException(_localizer["El campo Nombre es requerido para crear el Comportamiento."]);

            if (await _context.ComportamientosExpedientes.AnyAsync(ce => ce.Nombre.Equals(request.Nombre)))
                throw new BadRequestException(_localizer["El Nombre ingresado ya existe."]);

            if (request.RequisitosComportamientosExpedientes == null || !request.RequisitosComportamientosExpedientes.Any())
                throw new BadRequestException(_localizer["Debe seleccionar como mínimo un Requisito."]);

            if (request.NivelesUsoComportamientosExpedientes == null || !request.NivelesUsoComportamientosExpedientes.Any())
                throw new BadRequestException(_localizer["Debe seleccionar como mínimo un Nivel de Uso."]);
        }

        protected internal virtual ComportamientoExpediente AssignNewComportamientoExpediente(
            CreateComportamientoExpedienteCommand request, List<RequisitoExpediente> requisitos, 
            List<TipoRequisitoExpediente> tiposRequisitos, CancellationToken cancellationToken)
        {
            return new ComportamientoExpediente
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                EstaVigente = request.EstaVigente,
                RequisitosComportamientosExpedientes = request.RequisitosComportamientosExpedientes
                    .Select(rce => new RequisitoComportamientoExpediente
                    {
                        RequisitoExpediente = requisitos.First(r => r.Id == rce.RequisitoExpediente.Id),
                        TipoRequisitoExpediente = tiposRequisitos.First(r => r.Id == rce.TipoRequisitoExpediente.Id)
                    }).ToList(),
                NivelesUsoComportamientosExpedientes = request.NivelesUsoComportamientosExpedientes
                    .Select(async nuce => await _mediator.Send(new AddNivelUsoComportamientoExpedienteUncommitCommand(nuce), 
                        cancellationToken))
                    .Select(t => t.Result).ToList()
            };
        }
    }
}
