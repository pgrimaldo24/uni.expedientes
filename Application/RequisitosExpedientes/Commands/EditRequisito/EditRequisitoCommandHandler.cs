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

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.EditRequisito
{
    public class EditRequisitoCommandHandler : IRequestHandler<EditRequisitoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditRequisitoCommandHandler> _localizer;

        public EditRequisitoCommandHandler(IExpedientesContext context, IStringLocalizer<EditRequisitoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(EditRequisitoCommand request, CancellationToken cancellationToken)
        {
            await ValidatePropiedadesRequeridas(request, cancellationToken);
            var requisito = await _context.RequisitosExpedientes
                .Include(re => re.RequisitosExpedientesRequerimientosTitulos)
                .Include(re => re.RequisitosExpedientesFilesType)
                .Include(re => re.EstadoExpediente)
                .Include(re => re.RolesRequisitosExpedientes)
                .Include(re => re.RequisitosExpedientesDocumentos)
                .FirstOrDefaultAsync(re => re.Id == request.Id && !re.Bloqueado, cancellationToken);
            if (requisito == null)
                throw new NotFoundException(nameof(RequisitoExpediente), request.Id);

            AssignEditRequisito(request, requisito);
            await AssignRequerimientoTitulo(request, requisito, cancellationToken);
            await AssignEstadoExpedienteTitulo(request, requisito, cancellationToken);
            AssignRoles(request, requisito);
            AssignFilesTypes(request, requisito);
            UpdateDocumentosSecurizados(request.DocumentoSecurizado, requisito.RequisitosExpedientesDocumentos);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        protected internal virtual async Task ValidatePropiedadesRequeridas(EditRequisitoCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new BadRequestException(_localizer["El campo Nombre es requerido para editar el Requisito."]);

            if (request.EnviarEmailAlumno && string.IsNullOrWhiteSpace(request.Descripcion))
                throw new BadRequestException(_localizer["El campo Descripción es requerido para editar el Requisito."]);

            if (request.RequisitosExpedientesRequerimientosTitulos.RequiereMatricularse &&
                    request.RequisitosExpedientesRequerimientosTitulos.IdTipoRelacionExpediente == 0)
                throw new BadRequestException(_localizer["El campo Tipo Expediente es requerido para editar el Requisito."]);

            if (await _context.RequisitosExpedientes.AnyAsync(re 
                    => re.Id != request.Id && re.Nombre == request.Nombre, cancellationToken))
                throw new BadRequestException(_localizer[$"Ya existe un Requisito con el nombre: {request.Nombre}"]);
        }

        protected internal virtual void UpdateDocumentosSecurizados(bool documentoSecurizado, ICollection<RequisitoExpedienteDocumento> requisitosExpedientesDocumentos)
        {
            foreach (var item in requisitosExpedientesDocumentos)
                item.DocumentoSecurizado = documentoSecurizado;
        }

        protected internal virtual void AssignEditRequisito(EditRequisitoCommand request, 
            RequisitoExpediente requisitoExpediente)
        {
            requisitoExpediente.Nombre = request.Nombre;
            requisitoExpediente.Orden = request.Orden;
            requisitoExpediente.Descripcion = request.Descripcion;
            requisitoExpediente.EstaVigente = request.EstaVigente;
            requisitoExpediente.RequeridaParaTitulo = request.RequeridaParaTitulo;
            requisitoExpediente.RequiereDocumentacion = request.RequiereDocumentacion;
            requisitoExpediente.EnviarEmailAlumno = request.EnviarEmailAlumno;
            requisitoExpediente.RequeridaParaPago = request.RequeridaParaPago;
            requisitoExpediente.EstaRestringida = request.EstaRestringida;
            requisitoExpediente.EsCertificado = request.EsCertificado;
            requisitoExpediente.RequiereTextoAdicional = request.RequiereTextoAdicional;
            requisitoExpediente.IdRefModoRequerimientoDocumentacion = request.IdRefModoRequerimientoDocumentacion;
        }

        protected internal virtual async Task AssignRequerimientoTitulo(EditRequisitoCommand request,
            RequisitoExpediente requisitoExpediente, CancellationToken cancellationToken)
        {
            if (requisitoExpediente.RequisitosExpedientesRequerimientosTitulos.Any())
                _context.RequisitosExpedientesRequerimientosTitulos
                    .RemoveRange(requisitoExpediente.RequisitosExpedientesRequerimientosTitulos);

            if (request.RequisitosExpedientesRequerimientosTitulos.IdTipoRelacionExpediente == 0)
                return;

            var tipoRelacionExpediente = await _context.TiposRelacionesExpediente
                .FirstOrDefaultAsync(tre => tre.Id == request.RequisitosExpedientesRequerimientosTitulos.IdTipoRelacionExpediente,
                cancellationToken);
            if (tipoRelacionExpediente == null)
                throw new NotFoundException(nameof(TipoRelacionExpediente), request.Id);

            var requerimiento = new RequisitoExpedienteRequerimientoTitulo
            {
                RequiereMatricularse = request.RequisitosExpedientesRequerimientosTitulos.RequiereMatricularse,
                TipoRelacionExpediente = tipoRelacionExpediente
            };
            requisitoExpediente.RequisitosExpedientesRequerimientosTitulos.Add(requerimiento);
        }

        protected internal virtual async Task AssignEstadoExpedienteTitulo(EditRequisitoCommand request,
            RequisitoExpediente requisitoExpediente, CancellationToken cancellationToken)
        {
            var estadoExpediente = await _context.EstadosExpedientes
                .FirstOrDefaultAsync(ee => ee.Id == request.IdEstadoExpediente,
                cancellationToken);
            requisitoExpediente.EstadoExpediente = estadoExpediente;
        }

        protected internal virtual void AssignRoles(EditRequisitoCommand request,
            RequisitoExpediente requisitoExpediente)
        {
            requisitoExpediente.DeleteRolesNoIncluidos(request.Roles);
            if (request.Roles == null) return;

            var rolesAGuardar = request.Roles.Where(rol => requisitoExpediente.RolesRequisitosExpedientes
                                    .All(ra => ra.Rol != rol)).ToList();
            foreach (var rol in rolesAGuardar)
            {
                requisitoExpediente.RolesRequisitosExpedientes.Add(new RolRequisitoExpediente
                {
                    Rol = rol
                });
            }
        }

        protected internal virtual void AssignFilesTypes(EditRequisitoCommand request,
            RequisitoExpediente requisitoExpediente)
        {
            requisitoExpediente.DeleteFilesTypesNoIncluidos(request.IdsFilesTypes);
            if (request.IdsFilesTypes == null) return;

            var filesTypesAGuardar = request.IdsFilesTypes.Where(fileType => requisitoExpediente.RequisitosExpedientesFilesType
                                    .All(ra => ra.IdRefFileType != fileType)).ToList();
            foreach (var fileType in filesTypesAGuardar)
            {
                requisitoExpediente.RequisitosExpedientesFilesType.Add(new RequisitoExpedienteFileType
                {
                    IdRefFileType = fileType
                });
            }
        }
    }
}
