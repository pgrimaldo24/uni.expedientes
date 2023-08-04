using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Financiero;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/integracion/financiero")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class FinancieroController : ControllerSuperType
    {
        private readonly IFinancieroServiceClient _financieroServiceClient;

        public FinancieroController(IFinancieroServiceClient financieroServiceClient)
        {
            _financieroServiceClient = financieroServiceClient;
        }

        [Route("deuda-cliente")]
        [HttpPost]
        public async Task<ActionResult<DeudaClienteModel>>
            GetDeudaCliente(DeudaClienteParameters parameters)
        {
            var result =
                await _financieroServiceClient.GetDeudaCliente(parameters);
            return Ok(result);
        }
    }
}
