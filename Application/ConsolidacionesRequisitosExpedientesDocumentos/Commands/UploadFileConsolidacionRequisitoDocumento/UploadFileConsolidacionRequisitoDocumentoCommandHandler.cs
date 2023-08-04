using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.GestorDocumental;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Files;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UploadFileConsolidacionRequisitoDocumento
{
    public class UploadFileConsolidacionRequisitoDocumentoCommandHandler : IRequestHandler<UploadFileConsolidacionRequisitoDocumentoCommand, int>
    {
        private static readonly string RouteFile = $"{nameof(ConsolidacionRequisitoExpedienteDocumento)}/{nameof(ConsolidacionRequisitoExpedienteDocumento.Fichero)}/{{0}}{{1}}";

        private readonly IExpedientesContext _context;
        private readonly IFileManager _fileManager;
        private readonly IGestorDocumentalServiceClient _gestorDocumentalServiceClient;
        private readonly IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler> _localizer;

        public UploadFileConsolidacionRequisitoDocumentoCommandHandler(
            IExpedientesContext context,
            IFileManager fileManager,
            IGestorDocumentalServiceClient gestorDocumentalServiceClient,
            IStringLocalizer<UploadFileConsolidacionRequisitoDocumentoCommandHandler> localizer)
        {
            _context = context;
            _fileManager = fileManager;
            _gestorDocumentalServiceClient = gestorDocumentalServiceClient;
            _localizer = localizer;
        }

        public async Task<int> Handle(UploadFileConsolidacionRequisitoDocumentoCommand request, CancellationToken cancellationToken)
        {
            var consolidacion = await _context.ConsolidacionesRequisitosExpedientes
                .Include(cre => cre.ExpedienteAlumno)
                .Include(cre => cre.RequisitoExpediente)
                .FirstOrDefaultAsync(cre => cre.Id == request.IdConsolidacionRequisito, cancellationToken);
            if (consolidacion == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpediente), request.IdConsolidacionRequisito);

            var requisitoDocumento = await _context.RequisitosExpedientesDocumentos
                .FirstOrDefaultAsync(cre => cre.Id == request.IdRequisitoDocumento, cancellationToken);
            if (requisitoDocumento == null)
                throw new NotFoundException(nameof(RequisitoExpedienteDocumento), request.IdRequisitoDocumento);

            var configuracionUniversidad = await _context.ConfiguracionesExpedientesUniversidades
               .FirstOrDefaultAsync(ceu => ceu.IdRefUniversidad == consolidacion.ExpedienteAlumno.IdRefUniversidad);
            if (configuracionUniversidad == null)
                throw new NotFoundException(nameof(ConfiguracionExpedienteUniversidad), consolidacion.ExpedienteAlumno.IdRefUniversidad);

            ValidatePropiedades(configuracionUniversidad, request);

            var fileName = $"{request.File.FileName} ({requisitoDocumento.NombreDocumento})";
            var consolidacionRequisitoExpedienteDocumento = new ConsolidacionRequisitoExpedienteDocumento
            {
                Fichero = fileName,
                FechaFichero = DateTime.UtcNow,
                RequisitoExpedienteDocumentoId = request.IdRequisitoDocumento,
                ConsolidacionRequisitoExpedienteId = request.IdConsolidacionRequisito
            };

            await _context.ConsolidacionesRequisitosExpedientesDocumentos
                .AddAsync(consolidacionRequisitoExpedienteDocumento, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            try
            {
                var codigoClasificacion = $"{configuracionUniversidad.CodigoDocumental}.{requisitoDocumento.DocumentoClasificacion}";
                var documento = await GetDocumento(consolidacion, consolidacionRequisitoExpedienteDocumento, codigoClasificacion, request.File.FileName);
                consolidacionRequisitoExpedienteDocumento.IdRefDocumento = documento.Id;
                var fileBytes = ConvertToBytes(request.File);
                await WriteFileDocumento(fileBytes, consolidacionRequisitoExpedienteDocumento, request.File.FileName);
                await _context.SaveChangesAsync(cancellationToken);
                return consolidacionRequisitoExpedienteDocumento.Id;
            }
            catch (Exception ex)
            {
                _context.ConsolidacionesRequisitosExpedientesDocumentos.Remove(consolidacionRequisitoExpedienteDocumento);
                await _context.SaveChangesAsync(cancellationToken);
                throw new BadRequestException(_localizer[ex.Message]);
            }            
        }

        protected internal virtual void ValidatePropiedades(ConfiguracionExpedienteUniversidad configuracionUniversidad,
            UploadFileConsolidacionRequisitoDocumentoCommand request)
        {
            if (string.IsNullOrEmpty(configuracionUniversidad.CodigoDocumental))
                throw new BadRequestException(_localizer["No existe clasificación documental asociada a la universidad"]);

            if (!ConsolidacionRequisitoExpedienteDocumento.Extensiones.Contains(Path.GetExtension(request.File.FileName).Replace(".", "")))
                throw new BadRequestException(_localizer["Extensión de archivo no permitido"]);

            var fileSizeInMb = request.File.Length / ConsolidacionRequisitoExpedienteDocumento.Capacidad / ConsolidacionRequisitoExpedienteDocumento.Capacidad;
            if (fileSizeInMb > configuracionUniversidad.TamanyoMaximoFichero)
                throw new BadRequestException(_localizer[$"El tamaño máximo es de {configuracionUniversidad.TamanyoMaximoFichero} MB"]);
        }

        protected internal virtual async Task<DocumentoModel> GetDocumento(ConsolidacionRequisitoExpediente consolidacion,
            ConsolidacionRequisitoExpedienteDocumento consolidacionRequisitoExpedienteDocumento, string codigoClasificacion, string nombreArchivo)
        {
            var clasificiones = await _gestorDocumentalServiceClient.GetClasificaciones(new ClasificacionListParameters
            {
                FilterCodigo = codigoClasificacion
            });
            if (!clasificiones.Any())
                throw new BadRequestException(_localizer["No se encontró la clasificación en Gestor Documental"]);

            var expediente = consolidacion.ExpedienteAlumno;
            var descripcionAlumno = $"Alumno: {expediente.IdRefIntegracionAlumno}, Nº Identidad: {expediente.AlumnoNroDocIdentificacion}, " +
                $"{expediente.AlumnoNombre} {expediente.AlumnoApellido1} {expediente.AlumnoApellido2}";
            var descripcionPlan = $"Plan de Estudio: {expediente.IdRefPlan}, {expediente.NombrePlan?.Split(" - ")[0]}, '{expediente.NombrePlan?.Split(" - ")[1]}'";
            var descripcionDocumento = $"Documento: '{consolidacionRequisitoExpedienteDocumento.RequisitoExpedienteDocumento.NombreDocumento}', '{consolidacion.RequisitoExpediente.Nombre}'";
            var documentoParameters = new DocumentoParameters
            {
                Referencia = $"Expediente: {expediente.Id}\n{descripcionAlumno}\n{descripcionPlan}\n{descripcionDocumento}",
                Nombre = consolidacionRequisitoExpedienteDocumento.Fichero,
                Url = string.Format(RouteFile, consolidacionRequisitoExpedienteDocumento.Id, Path.GetExtension(nombreArchivo)),
                IdRefClasificacion = clasificiones.First().Id.ToString()
            };
            var documentoResponse = await _gestorDocumentalServiceClient.SaveDocumento(documentoParameters);
            if (documentoResponse.HasError)
                throw new BadRequestException(_localizer[documentoResponse.Error]);

            return documentoResponse;
        } 

        protected internal virtual byte[] ConvertToBytes(IFormFile file)
        {
            var reader = new BinaryReader(file.OpenReadStream());
            var bytes = reader.ReadBytes((int)file.Length);
            return bytes;
        }

        protected internal virtual async Task WriteFileDocumento(byte[] fileBytes,
          ConsolidacionRequisitoExpedienteDocumento consolidacionRequisitoExpedienteDocumento, string nombreArchivo)
        {
            var relativePath = string.Format(RouteFile, 
                consolidacionRequisitoExpedienteDocumento.Id, Path.GetExtension(nombreArchivo));
            if (!await _fileManager.WriteFileAsync(relativePath, fileBytes))
                throw new BadRequestException(_localizer["No se ha podido guardar el archivo"]);
        }
    }
}
