using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio
{
    public class CanQualifyAlumnoInPlanEstudioQueryHandler : IRequestHandler<CanQualifyAlumnoInPlanEstudioQuery, ExpedienteAlumnoTitulacionPlanDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler> _localizer;
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public CanQualifyAlumnoInPlanEstudioQueryHandler(IExpedientesContext context,
            IStringLocalizer<CanQualifyAlumnoInPlanEstudioQueryHandler> localizer,
            IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<ExpedienteAlumnoTitulacionPlanDto> Handle(CanQualifyAlumnoInPlanEstudioQuery request, CancellationToken cancellationToken)
        {
            var expedienteAlumno = await _context.ExpedientesAlumno.FirstOrDefaultAsync(
                ea => ea.IdRefIntegracionAlumno == request.IdRefIntegracionAlumno && ea.IdRefPlan == request.IdRefPlan,
                cancellationToken);
            if (expedienteAlumno == null)
                throw new BadRequestException(_localizer[
                    $"No se ha encontrado un Expediente con el IdPlan {request.IdRefPlan} y IdIntegracionAlumno {request.IdRefIntegracionAlumno}."]);

            var resultExpedientesGestor =
                await _expedientesGestorUnirServiceClient.GetExpedienteGestorFormatoComercialWithAsignaturasErp(
                    request.IdRefIntegracionAlumno, int.Parse(request.IdRefPlan));
            var resultIsPlanSuperado = await GetIsPlanSurpassed(expedienteAlumno, resultExpedientesGestor);
            var resultCausasFalloMatriculas = await 
                GetCausasFallosComprobacionMatriculasDocumentacionErp(expedienteAlumno.IdRefIntegracionAlumno, expedienteAlumno.IdRefPlan);

            resultIsPlanSuperado.MatriculasOk = resultCausasFalloMatriculas.MatriculasOk;
            resultIsPlanSuperado.CausasFalloMatriculas = resultCausasFalloMatriculas.CausasFallosMatriculas;
            resultIsPlanSuperado.PuedeTitular = resultIsPlanSuperado.MatriculasOk && resultIsPlanSuperado.EsPlanSuperado.EsSuperado;
            return resultIsPlanSuperado;
        }

        protected internal virtual async Task<ExpedienteAlumnoTitulacionPlanDto> GetIsPlanSurpassed(
            ExpedienteAlumno expedienteAlumno,
            ExpedienteErpComercialIntegrationModel expedienteErpComercialIntegrationModel)
        {
            var result = new ExpedienteAlumnoTitulacionPlanDto();
            var asignaturasSuperadas = GetAsignaturasSurpassedErpComercial(expedienteErpComercialIntegrationModel);
            if (asignaturasSuperadas.Any())
                return await GetPlanSurpassedErp(int.Parse(expedienteAlumno.IdRefPlan), expedienteAlumno,
                    asignaturasSuperadas);

            const string causaInsuperacionNoAsignaturasSuperadas = "El Alumno no tiene ninguna Asignatura Superada";
            result.EsPlanSuperado = new PlanSuperadoErpAcademicoModel
            {
                CausasInsuperacion = new List<string>
                {
                    causaInsuperacionNoAsignaturasSuperadas
                }
            };
            return result;
        }

        protected internal virtual List<AsignaturaErpComercialExpedientesIntegrationModel>
            GetAsignaturasSurpassedErpComercial(
                ExpedienteErpComercialIntegrationModel expedienteErpComercialIntegrationModel)
        {
            var asignaturasSuperadas = new List<AsignaturaErpComercialExpedientesIntegrationModel>();
            if (expedienteErpComercialIntegrationModel.Asignaturas == null ||
                !expedienteErpComercialIntegrationModel.Asignaturas.Any())
                return asignaturasSuperadas;

            return expedienteErpComercialIntegrationModel.Asignaturas
                .Where(a => a.IdAsignatura > 0 &&
                            (a.EsEstadoSuperada || a.EsEstadoMatriculaHonor || a.EsEstadoReconocida))
                .ToList();
        }

        protected internal virtual async Task<ExpedienteAlumnoTitulacionPlanDto> GetPlanSurpassedErp(int idPlan,
            ExpedienteAlumno expedienteAlumno,
            List<AsignaturaErpComercialExpedientesIntegrationModel> asignaturasSuperadas)
        {
            var filtersPlanSuperadoParameters = new EsPlanSuperadoParameters
            {
                IdNodo = int.Parse(expedienteAlumno.IdRefNodo),
                IdVersionPlan = !string.IsNullOrWhiteSpace(expedienteAlumno.IdRefVersionPlan)
                    ? int.Parse(expedienteAlumno.IdRefVersionPlan)
                    : null,
                IdsAsignaturasPlanes = asignaturasSuperadas.Select(a => a.IdAsignatura).ToList()
            };

            var resultElementosSuperadosPlan = await 
                _erpAcademicoServiceClient.ItIsPlanSurpassed(idPlan, filtersPlanSuperadoParameters);
            return new ExpedienteAlumnoTitulacionPlanDto
            {
                EsPlanSuperado = resultElementosSuperadosPlan
            };
        }

        protected internal virtual async Task<ValidateAlumnoMatriculacionErpAcademicoModel>
            GetCausasFallosComprobacionMatriculasDocumentacionErp(string idIntegracionAlumno, string idRefPlan)
        {
            var parameters = new ValidateAlumnoMatriculacionParameters
            {
                IdIntegracionAlumno = idIntegracionAlumno,
                IdRefPlan = idRefPlan
            };
            return await 
                _erpAcademicoServiceClient.ValidateAlumnoMatriculacion(parameters);
        }
    }
}
