using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Anotaciones.Commands.CreateAnotacion;
using Unir.Expedientes.Application.Anotaciones.Commands.DeleteAnotacion;
using Unir.Expedientes.Application.Anotaciones.Commands.EditAnotacion;
using Unir.Expedientes.Application.Anotaciones.Queries.GetAnotacionById;
using Unir.Expedientes.Application.Anotaciones.Queries.GetPagedAnotaciones;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;
using Unir.Framework.WebApiSuperTypes.Models.Cal;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/anotaciones")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    [ApiController]
    public class AnotacionesController : ControllerSuperType
    {
        [Route("query")]
        [HttpPost]
        public async Task<ActionResult<PagedListResponseMessage<AnotacionListItemDto>>> GetAnotacionesAsync(AnotacionesCriteriaRequest criteria)
        {
            var result = await Mediator.Send(criteria.GetQueryParameters());
            var response = new PagedListResponseMessage<AnotacionListItemDto>
            {
                Data = result.Elements,
                Meta = new MetaResponse
                {
                    TotalCount = result.TotalElements
                }
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAnotacionesAsync(CreateAnotacionCommand request)
        {
            await Mediator.Send(request);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> EditAnotacionesAsync(EditAnotacionCommand request)
        {
            await Mediator.Send(request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnotacionesAsync(int id)
        {
            await Mediator.Send(new DeleteAnotacionCommand(id));
            return NoContent();
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> GetAnotacionesByIdAsync(int id)
        {
            return Ok(await Mediator.Send(new GetAnotacionByIdQuery(id)));
        }
    }
}
