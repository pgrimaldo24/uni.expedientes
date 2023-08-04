using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Commands.CreateTipoSituacionEstadoExpediente;
using Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Queries.GetUltimoTipoSituacionEstadoByIdExpediente;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/tipos-situacion-estado-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    [ApiController]
    public class TiposSituacionEstadoExpedientesController : ControllerSuperType
    {
        [HttpPost]
        public async Task<IActionResult> CreateTipoSituacionEstadoExpedienteAsync(
            CreateTipoSituacionEstadoExpedienteCommand request)
        {
            await Mediator.Send(request);
            return Ok();
        }

        [Route("expediente/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetUltimoTipoSituacionEstadoByIdExpedienteAsync(int id)
        {
            var result = await Mediator.Send(new GetUltimoTipoSituacionEstadoByIdExpedienteQuery(id));
            return Ok(result);
        }
    }
}
