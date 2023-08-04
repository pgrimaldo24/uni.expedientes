using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionesExpedientesBackground;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/asignaturas-calificaciones")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    [ApiController]
    public class AsignaturasCalificacionController : ControllerSuperType
    {
        [Route("migrar-calificaciones-expedientes")]
        [HttpPost]
        public async Task<IActionResult> MigrarCalificacionesExpedientesBackgroundAsync(
            MigrarCalificacionesExpedientesBackgroundCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }
    }
}
