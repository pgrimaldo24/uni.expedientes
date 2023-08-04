using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.CreateRequisito;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisito;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisitosMasivo;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.EditRequisito;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetRequisitoById;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetTiposRequisitosExpedientes;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;
using Unir.Framework.WebApiSuperTypes.Models.Cal;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/requisitos-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class RequisitosExpedientesController : ControllerSuperType
    {
        [Route("query")]
        [HttpPost]
        public async Task<ActionResult<PagedListResponseMessage<RequisitosExpedientesListItemDto>>> GetPagedRequisitosExpedientesAsync(
            RequisitosExpedientesCriteriaRequest criteria)
        {
            var result = await Mediator.Send(criteria.GetQueryParameters());
            var response = new PagedListResponseMessage<RequisitosExpedientesListItemDto>
            {
                Data = result.Elements,
                Meta = new MetaResponse
                {
                    TotalCount = result.TotalElements
                }
            };
            return Ok(response);
        }

        [Route("query-all")]
        [HttpPost]
        public async Task<ActionResult<RequisitosExpedientesListItemDto[]>> GetAllRequisitosExpedientesAsync(
        RequisitosExpedientesAllCriteriaRequest criteria)
        {
            criteria.Offset = criteria.Limit * (criteria.Offset - 1);
            var result = await Mediator.Send(criteria.GetQueryParameters());
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRequisitoAsync(int id)
        {
            await Mediator.Send(new DeleteRequisitoCommand(id));
            return NoContent();
        }

        [HttpPost("delete-massive")]
        public async Task<ActionResult> DeleteRequisitosMasivoAsync(
            DeleteRequisitosMasivoCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRequisitoAsync(CreateRequisitoCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult> EditRequisitoAsync(EditRequisitoCommand request)
        {
            await Mediator.Send(request);
            return NoContent();
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> GetRequisitoByIdAsync(int id)
        {
            return Ok(await Mediator.Send(new GetRequisitoByIdQuery(id)));
        }

        [Route("tipos-requisitos/query")]
        [HttpPost]
        public async Task<ActionResult<TipoRequisitoExpedienteListItemDto[]>> GetTiposRequisitosExpedientesAsync(
        TiposRequisitosExpedientesCriteriaRequest criteria)
        {
            criteria.Offset = criteria.Limit * (criteria.Offset - 1);
            var result = await Mediator.Send(criteria.GetQueryParameters());
            return Ok(result);
        }

        [Route("query-bloqueados")]
        [HttpPost]
        public async Task<ActionResult<RequisitosExpedientesListItemDto[]>> GetRequisitosExpedientesBloqueadosAsync(
            RequisitosExpedientesBloqueadosCriteriaRequest criteria)
        {
            criteria.Offset = criteria.Limit * (criteria.Offset - 1);
            var result = await Mediator.Send(criteria.GetQueryParameters());
            return Ok(result);
        }
    }
}