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

namespace Unir.Expedientes.Application.Anotaciones.Commands.EditAnotacion
{
    public class EditAnotacionCommandHandler : IRequestHandler<EditAnotacionCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditAnotacionCommandHandler> _localizer;
        private readonly IIdentityService _identityService;

        public EditAnotacionCommandHandler(IExpedientesContext context, 
            IStringLocalizer<EditAnotacionCommandHandler> localizer,
            IIdentityService identityService)
        {
            _context = context;
            _localizer = localizer;
            _identityService = identityService;
        }

        public async Task<Unit> Handle(EditAnotacionCommand request, CancellationToken cancellationToken)
        {
            var resultValidatePropiedadesRequeridas = ValidatePropiedadesRequeridas(request);
            if (resultValidatePropiedadesRequeridas != PropiedadesRequeridas.Ninguno)
            {
                throw new BadRequestException(_localizer[$"El campo {resultValidatePropiedadesRequeridas} es requerido para crear la Observación."]);
            }

            var anotacion = await _context.Anotaciones
                .Include(a => a.RolesAnotaciones)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (anotacion == null)
                throw new NotFoundException(nameof(Anotacion), request.Id);

            AssignEditAnotacion(request, anotacion);
            var expedienteAlumno = await _context.ExpedientesAlumno
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            var infoSecurity = _identityService.GetUserIdentityInfo();
            if (infoSecurity == null)
                throw new BadRequestException(_localizer["Usuario no encontrado."]);

            anotacion.ExpedienteAlumno = expedienteAlumno;
            anotacion.IdRefCuentaSeguridadModificacion = infoSecurity.Id;
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        protected internal virtual void AssignEditAnotacion(EditAnotacionCommand request, Anotacion anotacion)
        {
            anotacion.EsPublica = request.EsPublica;
            anotacion.EsRestringida = request.EsRestringida;
            anotacion.Mensaje = request.Mensaje;
            anotacion.Resumen = request.Resumen;
            anotacion.FechaModificacion = DateTime.UtcNow;
            AddRoles(anotacion, request.RolesAnotaciones);
        }

        protected internal virtual void AddRoles(Anotacion anotacion, string[] roles)
        {
            anotacion.DeleteRolesNoIncluidos(roles);
            if (roles == null) return;

            var rolesAGuardar = roles.Where(rol => anotacion.RolesAnotaciones.All(ra => ra.Rol != rol)).ToList();
            foreach (var rol in rolesAGuardar)
            {
                anotacion.RolesAnotaciones.Add(new RolAnotacion
                {
                    Rol = rol
                });
            }
        }

        protected internal virtual PropiedadesRequeridas ValidatePropiedadesRequeridas(
            EditAnotacionCommand request)
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
