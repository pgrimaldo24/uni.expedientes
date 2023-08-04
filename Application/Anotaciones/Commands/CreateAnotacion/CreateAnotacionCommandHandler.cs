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
using Unir.Framework.Crosscutting.Security;

namespace Unir.Expedientes.Application.Anotaciones.Commands.CreateAnotacion
{
    public class CreateAnotacionCommandHandler : IRequestHandler<CreateAnotacionCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CreateAnotacionCommandHandler> _localizer;
        private readonly IIdentityService _identityService;

        public CreateAnotacionCommandHandler(IExpedientesContext context, 
            IStringLocalizer<CreateAnotacionCommandHandler> localizer,
            IIdentityService identityService)
        {
            _context = context;
            _localizer = localizer;
            _identityService = identityService;
        }

        public async Task<Unit> Handle(CreateAnotacionCommand request, CancellationToken cancellationToken)
        {
            var resultValidatePropiedadesRequeridas = ValidatePropiedadesRequeridas(request);
            if (resultValidatePropiedadesRequeridas != PropiedadesRequeridas.Ninguno)
                throw new BadRequestException(_localizer[$"El campo {resultValidatePropiedadesRequeridas} es requerido para crear la Observación."]);

            var newAnotacion = AssignNewAnotacion(request);
            var expedienteAlumno = await _context.ExpedientesAlumno
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            var infoSecurity = _identityService.GetUserIdentityInfo();
            if (infoSecurity == null)
                throw new BadRequestException(_localizer["Usuario no encontrado."]);

            newAnotacion.ExpedienteAlumno = expedienteAlumno;
            newAnotacion.IdRefCuentaSeguridad = infoSecurity.Id;
            newAnotacion.IdRefCuentaSeguridadModificacion = infoSecurity.Id;

            await _context.Anotaciones.AddAsync(newAnotacion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        protected internal virtual Anotacion AssignNewAnotacion(CreateAnotacionCommand request)
        {
            return new()
            {
                Fecha = DateTime.UtcNow,
                EsPublica = request.EsPublica,
                EsRestringida = request.EsRestringida,
                Mensaje = request.Mensaje,
                Resumen = request.Resumen,
                FechaModificacion = DateTime.UtcNow,
                RolesAnotaciones = request.RolesAnotaciones?.Select(rol => new RolAnotacion
                {
                    Rol = rol
                }).ToList()
            };
        }

        protected internal virtual PropiedadesRequeridas ValidatePropiedadesRequeridas(
            CreateAnotacionCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.Resumen))
                return PropiedadesRequeridas.Resumen;

            if (request.EsRestringida && (request.RolesAnotaciones == null || !request.RolesAnotaciones.Any()))
                return PropiedadesRequeridas.ListaRoles;

            return PropiedadesRequeridas.Ninguno;
        }
    }

    public enum PropiedadesRequeridas
    {
        Ninguno,
        Resumen,
        ListaRoles
    }
}
