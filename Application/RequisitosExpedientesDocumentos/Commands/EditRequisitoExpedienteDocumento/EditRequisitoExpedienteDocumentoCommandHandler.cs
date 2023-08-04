using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.EditRequisitoExpedienteDocumento
{
    public class EditRequisitoExpedienteDocumentoCommandHandler : IRequestHandler<EditRequisitoExpedienteDocumentoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler> _localizer;

        public EditRequisitoExpedienteDocumentoCommandHandler(IExpedientesContext context, IStringLocalizer<EditRequisitoExpedienteDocumentoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(EditRequisitoExpedienteDocumentoCommand request, CancellationToken cancellationToken)
        {
            var resultValidatePropiedadesRequeridas = ValidatePropiedadesRequeridas(request);
            if (resultValidatePropiedadesRequeridas != PropiedadesRequeridas.Ninguno)
                throw new BadRequestException(_localizer[$"El campo {resultValidatePropiedadesRequeridas} es requerido para editar el Documento."]);
             
            var existeDocumento = await _context.RequisitosExpedientesDocumentos
                .FirstOrDefaultAsync(red => red.Id != request.Id && red.RequisitoExpediente.Id == request.IdRequisitoExpediente
                    && red.NombreDocumento == request.NombreDocumento, cancellationToken);
            if (existeDocumento != null)
                throw new BadRequestException(_localizer[$"Ya existe un Documento con el nombre: {request.NombreDocumento}"]);

            var requisitoExpedienteDocumento = await _context.RequisitosExpedientesDocumentos
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (requisitoExpedienteDocumento == null)
                throw new NotFoundException(nameof(RequisitoExpedienteDocumento), request.Id);

            AssignEditRequisitoExpedienteDocumento(request, requisitoExpedienteDocumento);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        protected internal virtual void AssignEditRequisitoExpedienteDocumento(EditRequisitoExpedienteDocumentoCommand request, 
            RequisitoExpedienteDocumento requisitoExpedienteDocumento)
        {
            requisitoExpedienteDocumento.NombreDocumento = request.NombreDocumento;
            requisitoExpedienteDocumento.DocumentoEditable = request.DocumentoEditable;
            requisitoExpedienteDocumento.DocumentoObligatorio = request.DocumentoObligatorio;
            requisitoExpedienteDocumento.RequiereAceptacionAlumno = request.RequiereAceptacionAlumno;
            requisitoExpedienteDocumento.DocumentoSecurizado = request.DocumentoSecurizado;
        }

        protected internal virtual PropiedadesRequeridas ValidatePropiedadesRequeridas(
            EditRequisitoExpedienteDocumentoCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.NombreDocumento))
                return PropiedadesRequeridas.NombreDocumento;

            return PropiedadesRequeridas.Ninguno;
        }

        public enum PropiedadesRequeridas
        {
            Ninguno,
            NombreDocumento
        }
    }
}
