using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditComportamientoExpediente
{
    public class EditComportamientoExpedienteCommandHandler : IRequestHandler<EditComportamientoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditComportamientoExpedienteCommandHandler> _localizer;

        public EditComportamientoExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<EditComportamientoExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(EditComportamientoExpedienteCommand request, CancellationToken cancellationToken)
        {
            await ValidatePropiedadesRequeridas(request);
            var comportamientoExpediente = await _context.ComportamientosExpedientes
                .FirstOrDefaultAsync(ea => ea.Id == request.IdComportamiento, cancellationToken);
            if (comportamientoExpediente == null)
                throw new NotFoundException(nameof(ComportamientoExpediente), request.IdComportamiento);

            AssignComportamientoExpediente(request, comportamientoExpediente);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual async Task ValidatePropiedadesRequeridas(EditComportamientoExpedienteCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException(_localizer["El campo Nombre es requerido para editar el Comportamiento."]);

            if (await _context.ComportamientosExpedientes.AnyAsync(ce => 
                        ce.Nombre.Equals(request.Nombre) && ce.Id != request.IdComportamiento))
                throw new BadRequestException(_localizer["El Nombre ingresado ya existe."]);
        }

        protected internal virtual void AssignComportamientoExpediente(
            EditComportamientoExpedienteCommand request, ComportamientoExpediente comportamiento)
        {
            comportamiento.Nombre = request.Nombre;
            comportamiento.Descripcion = request.Descripcion;
            comportamiento.EstaVigente = request.EstaVigente;
        }
    }
}
