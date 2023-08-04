using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientes;
using Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientesByIdExpediente;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;
using Unir.Framework.WebApiSuperTypes.Models.Cal;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/seguimientos-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class SeguimientosExpedientesController : ControllerSuperType
    {
        [Route("query")]
        [HttpPost]
        public async Task<ActionResult<PagedListResponseMessage<SeguimientoExpedienteListItemDto>>>
            GetPagedSeguimientosExpedientesAsync(SeguimientoExpedienteCriteriaRequest criteria)
        {
            var result = await Mediator.Send(criteria.GetQueryParameters());
            var response = new PagedListResponseMessage<SeguimientoExpedienteListItemDto>
            {
                Data = result.Elements,
                Meta = new MetaResponse
                {
                    TotalCount = result.TotalElements
                }
            };

            return Ok(response);
        }

        [Route("{idExpediente:int}/query")]
        [HttpPost]
        public async Task<ActionResult<PagedListResponseMessage<SeguimientoExpedienteByIdExpedienteListItemDto>>>
            GetPagedSeguimientosExpedientesSinIntegracionAsync(
                SeguimientoExpedienteByIdExpedienteCriteriaRequest criteria, int idExpediente)
        {
            var criteriaRequest = criteria.GetQueryParameters();
            criteriaRequest.FilterIdExpedienteAlumno = idExpediente;
            var result = await Mediator.Send(criteriaRequest);
            var response = new PagedListResponseMessage<SeguimientoExpedienteByIdExpedienteListItemDto>
            {
                Data = result.Elements,
                Meta = new MetaResponse
                {
                    TotalCount = result.TotalElements
                }
            };

            return Ok(response);
        }
    }


}
