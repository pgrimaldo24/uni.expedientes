using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.HitosConseguidos.Queries.GetHitosConseguidosByIdExpediente;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/hitos-conseguidos")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    [ApiController]
    public class HitosConseguidosController : ControllerSuperType
    {
        [Route("expediente/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetHitosConseguidosByIdExpedienteAsync(int id)
        {
            var result = await Mediator.Send(new GetHitosConseguidosByIdExpedienteQuery(id));
            return Ok(result);
        }
    }
}
