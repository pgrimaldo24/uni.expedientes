using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/integracion/gestor-mapeos")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class GestorMapeosController : ControllerSuperType
    {
        private readonly IGestorMapeosServiceClient _gestorMapeosServiceClient;
        public GestorMapeosController(IGestorMapeosServiceClient gestorMapeosServiceClient)
        {
            _gestorMapeosServiceClient = gestorMapeosServiceClient;
        }

        [Route("estudios")]
        [HttpGet]
        public async Task<IActionResult> GetEstudiosAsync(string idRefPlan, string idRefVersionPlan)
        {
            var result = await _gestorMapeosServiceClient.GetEstudios(idRefPlan, idRefVersionPlan);
            return Ok(result);
        }

        [Route("asignaturas")]
        [HttpGet]
        public async Task<IActionResult> GetAsignaturasAsync(int idEstudioGestor)
        {
            var result = await _gestorMapeosServiceClient.GetAsignaturas(idEstudioGestor);
            return Ok(result);
        }
    }
}
