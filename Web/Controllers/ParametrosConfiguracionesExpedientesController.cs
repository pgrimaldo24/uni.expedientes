using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unir.Expedientes.Application.ParametrosConfiguracionesExpedientes.Queries.GetFirstParametrosConfiguracionExpediente;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/parametros-configuraciones-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ParametrosConfiguracionesExpedientesController : ControllerSuperType
    {
        [Route("condicion-8")]
        [HttpGet]
        public async Task<ActionResult<ParametroConfiguracionExpedienteFirstItemDto>>
            GetParametrosConfiguracionExpedienteCondicion8()
        {
            var result = await Mediator.Send(new GetFirstParametrosConfiguracionExpedienteQuery());
            return Ok(result);
        }

        [Route("primer-parametro")]
        [HttpGet]
        public async Task<ActionResult<ParametroConfiguracionExpedienteFirstItemDto>>
            GetPrimerParametroConfiguracionExpedienteAsync()
        {
            var result = await Mediator.Send(new GetFirstParametrosConfiguracionExpedienteQuery());
            return Ok(result);
        }
    }
}
