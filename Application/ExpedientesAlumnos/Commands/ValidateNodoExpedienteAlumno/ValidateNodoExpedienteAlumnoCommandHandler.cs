using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno
{
    public class ValidateNodoExpedienteAlumnoCommandHandler : IRequestHandler<ValidateNodoExpedienteAlumnoCommand, Unit>
    {
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler> _localizer;

        public ValidateNodoExpedienteAlumnoCommandHandler(IErpAcademicoServiceClient erpAcademicoServiceClient,
            IStringLocalizer<ValidateNodoExpedienteAlumnoCommandHandler> localizer)
        {
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(ValidateNodoExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            if (request.ExpedienteAlumno?.IdRefVersionPlan == null || request.ExpedienteAlumno?.IdRefNodo == null)
                return Unit.Value;

            var idNodo = int.Parse(request.ExpedienteAlumno.IdRefNodo);
            var nodoErp = await _erpAcademicoServiceClient.GetNodo(idNodo);
            if (nodoErp == null)
                throw new NotFoundException(nameof(NodoErpAcademicoModel), request.ExpedienteAlumno.IdRefNodo);

            var idsVersiones = nodoErp.VersionesPlanes?.Select(v => v.Id).ToList() ?? new List<int>();
            if (!idsVersiones.Any() ||
                idsVersiones.All(v => v.ToString() != request.ExpedienteAlumno.IdRefVersionPlan))
                throw new BadRequestException(_localizer["La Versión del Plan vinculada al Expediente no está asociada al Nodo de Inicio."]);

            return Unit.Value;
        }
    }
}
