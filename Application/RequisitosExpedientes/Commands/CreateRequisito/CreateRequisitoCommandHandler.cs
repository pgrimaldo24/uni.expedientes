using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.CreateRequisito
{
    public class CreateRequisitoCommandHandler : IRequestHandler<CreateRequisitoCommand, int>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CreateRequisitoCommandHandler> _localizer;

        public CreateRequisitoCommandHandler(IExpedientesContext context, IStringLocalizer<CreateRequisitoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<int> Handle(CreateRequisitoCommand request, CancellationToken cancellationToken)
        {
            var resultValidatePropiedadesRequeridas = ValidatePropiedadesRequeridas(request);
            if (resultValidatePropiedadesRequeridas != PropiedadesRequeridas.Ninguno)
                throw new BadRequestException(_localizer[$"El campo {resultValidatePropiedadesRequeridas} es requerido para crear el Requisito."]);

            var existeDocumento = await _context.RequisitosExpedientes
                .FirstOrDefaultAsync(re => re.Nombre == request.Nombre, cancellationToken);
            if (existeDocumento != null)
                throw new BadRequestException(_localizer[$"Ya existe un Requisito con el nombre: {request.Nombre}"]);

            var newRequisito = AssignNewRequisito(request);

            await _context.RequisitosExpedientes.AddAsync(newRequisito, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newRequisito.Id;
        }

        protected internal virtual RequisitoExpediente AssignNewRequisito(CreateRequisitoCommand request)
        {
            return new()
            {
                Nombre = request.Nombre,
                Orden = request.Orden
            };
        }

        protected internal virtual PropiedadesRequeridas ValidatePropiedadesRequeridas(
            CreateRequisitoCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                return PropiedadesRequeridas.Nombre;
            
            if (request.Orden <= 0)
                return PropiedadesRequeridas.Orden;

            return PropiedadesRequeridas.Ninguno;
        }

        public enum PropiedadesRequeridas
        {
            Ninguno,
            Nombre,
            Orden
        }
    }
}
