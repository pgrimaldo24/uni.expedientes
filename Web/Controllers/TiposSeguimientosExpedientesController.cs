using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;
using Unir.Expedientes.Application.TiposSeguimientosExpedientes.Queries.GetTiposSeguimientosExpedientes;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/tipos-seguimientos-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class TiposSeguimientosExpedientesController : ControllerSuperType
    {
        [Route("query")]
        [HttpPost]
        public async Task<ActionResult<TipoSeguimientoExpedienteListItemDto[]>> GetTiposSeguimientosExpedientesAsync(
            TipoSeguimientoExpedienteCriteriaRequest criteria)
        {
            criteria.Offset = criteria.Limit * (criteria.Offset - 1);
            var result = await Mediator.Send(criteria.GetQueryParameters());
            return Ok(result);
        }
    }
}
