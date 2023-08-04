using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.UpdateConsolidacionRequisitoExpediente
{
    public class UpdateConsolidacionRequisitoExpedienteCommandHandler : IRequestHandler<UpdateConsolidacionRequisitoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<UpdateConsolidacionRequisitoExpedienteCommandHandler> _localizer;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public UpdateConsolidacionRequisitoExpedienteCommandHandler(
            IExpedientesContext context, 
            IStringLocalizer<UpdateConsolidacionRequisitoExpedienteCommandHandler> localizer,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<Unit> Handle(UpdateConsolidacionRequisitoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var consolidacionRequisito = await _context.ConsolidacionesRequisitosExpedientes
                .Include(cre => cre.RequisitoExpediente)
                .Include(cre => cre.ConsolidacionesRequisitosExpedientesDocumentos)
                .FirstOrDefaultAsync(cre => cre.Id == request.Id);
            if (consolidacionRequisito == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpediente), request.Id);

            await ValidatePropiedades(consolidacionRequisito, request);

            var idioma = !string.IsNullOrWhiteSpace(request.IdRefIdioma) 
                ? await _erpAcademicoServiceClient.GetIdioma(Convert.ToInt32(request.IdRefIdioma))
                : null;

            AssignConsolidacionRequisitoExpediente(consolidacionRequisito, request, idioma);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual async Task ValidatePropiedades(ConsolidacionRequisitoExpediente consolidacionRequisito,
            UpdateConsolidacionRequisitoExpedienteCommand request)
        {
            var errorMessage = consolidacionRequisito.VerificarPropiedadesParaActualizar(
                request.Texto, request.IdRefIdioma, request.NivelIdioma);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new BadRequestException(_localizer[errorMessage]);

            if (request.IdCausaEstadoRequisito != null 
                && !await _context.CausasEstadosRequisitosConsolidadasExpedientes
                    .AnyAsync(cerce => cerce.Id == request.IdCausaEstadoRequisito))
                throw new BadRequestException(_localizer["No existe la causa del estado del requisito"]);
        }

        protected internal virtual void AssignConsolidacionRequisitoExpediente(ConsolidacionRequisitoExpediente consolidacionRequisito,
            UpdateConsolidacionRequisitoExpedienteCommand request, IdiomaAcademicoModel idioma)
        {
            consolidacionRequisito.EsDocumentacionFisica = request.EsDocumentacionFisica;
            consolidacionRequisito.EnviadaPorAlumno = request.EnviadaPorAlumno;            
            consolidacionRequisito.Texto = request.Texto;
            consolidacionRequisito.NivelIdioma = request.NivelIdioma;
            consolidacionRequisito.IdRefIdioma = idioma?.Id.ToString();
            consolidacionRequisito.NombreIdioma = idioma?.Nombre;
            consolidacionRequisito.SiglasIdioma = idioma?.Siglas;
            consolidacionRequisito.FechaCambioEstado = DateTime.UtcNow;
            consolidacionRequisito.EstadoRequisitoExpedienteId = EstadoRequisitoExpediente.Pendiente;
            consolidacionRequisito.DocumentacionRecibida = consolidacionRequisito.EsDocumentacionFisica
                || consolidacionRequisito.ConsolidacionesRequisitosExpedientesDocumentos.Any();
            consolidacionRequisito.Fecha = consolidacionRequisito.RequisitoExpediente.EsLogro
                ? request.Fecha : consolidacionRequisito.Fecha;
            consolidacionRequisito.CausaEstadoRequisitoConsolidadaExpedienteId = request.IdCausaEstadoRequisito;
        }
    }
}
