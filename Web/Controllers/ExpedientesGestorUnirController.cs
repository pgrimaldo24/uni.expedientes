using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/integracion/expedientes-gestor-unir")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ExpedientesGestorUnirController : ControllerSuperType
    {
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;

        public ExpedientesGestorUnirController(IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient)
        {
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
        }

        [Route("asignaturas-formato-comercial")]
        [HttpGet]
        public async Task<ActionResult<ExpedienteErpComercialIntegrationModel>> GetExpedienteFromServiciosGestorUnir(string idIntegracionAlumno, int idPlan)
        {
            var result =
                await _expedientesGestorUnirServiceClient.GetExpedienteGestorFormatoComercialWithAsignaturasErp(idIntegracionAlumno,
                    idPlan);
            return Ok(result);
        }

        [Route("asignaturas-formato-academico-completo")]
        [HttpGet]
        public async Task<ActionResult<ExpedienteExpedientesIntegrationModel>> GetExpedienteFromServiciosGestorAcademicoUnir(string idIntegracionAlumno, int idPlan)
        {
            var result =
                await _expedientesGestorUnirServiceClient.GetExpedienteGestorFormatoErpWithAsignaturas(idIntegracionAlumno,
                    idPlan);
            return Ok(result);
        }

        [Route("reconocimientos")]
        [HttpGet]
        public async Task<IActionResult> GetReconocimientosFromServiciosGestorUnir(
            string idIntegracionAlumno, int idEstudio)
        {
            var result = await _expedientesGestorUnirServiceClient
                .GetReconocimientos(idIntegracionAlumno, idEstudio);
            return Ok(result);
        }
    }
}
