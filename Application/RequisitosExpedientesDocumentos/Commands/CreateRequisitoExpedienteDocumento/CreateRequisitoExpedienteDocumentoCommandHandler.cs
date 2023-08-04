using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.CreateRequisitoExpedienteDocumento
{
    public class CreateRequisitoExpedienteDocumentoCommandHandler : IRequestHandler<CreateRequisitoExpedienteDocumentoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler> _localizer;

        public CreateRequisitoExpedienteDocumentoCommandHandler(IExpedientesContext context, IStringLocalizer<CreateRequisitoExpedienteDocumentoCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(CreateRequisitoExpedienteDocumentoCommand request, CancellationToken cancellationToken)
        {
            var resultValidatePropiedadesRequeridas = ValidatePropiedadesRequeridas(request);
            if (resultValidatePropiedadesRequeridas != PropiedadesRequeridas.Ninguno)
                throw new BadRequestException(_localizer[$"El campo {resultValidatePropiedadesRequeridas} es requerido para crear el Documento."]);

            var newRequisitoExpedienteDocumento = AssignNewRequisitoExpedienteDocumento(request);
            var requisitoExpediente = await _context.RequisitosExpedientes
                .FirstOrDefaultAsync(re => re.Id == request.IdRequisitoExpediente, cancellationToken);
            if (requisitoExpediente == null)
                throw new NotFoundException(nameof(RequisitoExpediente), request.IdRequisitoExpediente);

            var existeDocumento = await _context.RequisitosExpedientesDocumentos
                .FirstOrDefaultAsync(red => red.RequisitoExpediente.Id == request.IdRequisitoExpediente 
                && red.NombreDocumento == request.NombreDocumento, cancellationToken);
            if (existeDocumento != null)
                throw new BadRequestException(_localizer[$"Ya existe un Documento con el nombre: {request.NombreDocumento}"]);

            newRequisitoExpedienteDocumento.RequisitoExpediente = requisitoExpediente;

            await _context.RequisitosExpedientesDocumentos.AddAsync(newRequisitoExpedienteDocumento, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        protected internal virtual RequisitoExpedienteDocumento AssignNewRequisitoExpedienteDocumento(CreateRequisitoExpedienteDocumentoCommand request)
        {
            const string documentoClasificacion = "J.03.01.01";
            return new RequisitoExpedienteDocumento
            {
                NombreDocumento = request.NombreDocumento,
                DocumentoEditable = request.DocumentoEditable,
                DocumentoObligatorio = request.DocumentoObligatorio,
                RequiereAceptacionAlumno = request.RequiereAceptacionAlumno,
                DocumentoSecurizado = request.DocumentoSecurizado,
                DocumentoClasificacion = documentoClasificacion
            };
        }

        protected internal virtual PropiedadesRequeridas ValidatePropiedadesRequeridas(
            CreateRequisitoExpedienteDocumentoCommand request)
        {
            return string.IsNullOrWhiteSpace(request.NombreDocumento) 
                ? PropiedadesRequeridas.NombreDocumento : PropiedadesRequeridas.Ninguno;
        }

        public enum PropiedadesRequeridas
        {
            Ninguno,
            NombreDocumento
        }
    }
}
