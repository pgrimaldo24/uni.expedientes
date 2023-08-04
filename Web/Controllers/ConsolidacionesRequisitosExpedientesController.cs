using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.CreateConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.DeleteConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.RechazarConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.UpdateConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.ValidarConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetCausasEstadosRequisitosConsolidadasByIdRequisito;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionesRequisitosByIdExpediente;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/consolidaciones-requisitos-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ConsolidacionesRequisitosExpedientesController : ControllerSuperType
    {
        [Route("causas-query")]
        [HttpPost]
        public async Task<ActionResult<CausaEstadoRequisitoConsolidadaExpedienteListItemDto[]>> GetCausasEstadosRequisitosConsolidadasAsync(
            CausasEstadosRequisitosConsolidadasCriteriaRequest criteria)
        {
            criteria.Offset = criteria.Limit * (criteria.Offset - 1);
            var result = await Mediator.Send(criteria.GetQueryParameters());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConsolidacionRequisitoExpedienteAsync(
            CreateConsolidacionRequisitoExpedienteCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<ActionResult<ConsolidacionRequisitoExpedienteDto>>
            GetConsolidacionRequisitoExpedienteByIdAsync(int id)
        {
            var result = await Mediator.Send(new GetConsolidacionRequisitoExpedienteByIdQuery(id));
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateConsolidacionRequisitoExpedienteAsync(int id,
            UpdateConsolidacionRequisitoExpedienteCommand request)
        {
            request.Id = id;
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsolidacionRequisitoExpedienteAsync(int id)
        {
            await Mediator.Send(new DeleteConsolidacionRequisitoExpedienteCommand(id));
            return NoContent();
        }

        [Route("{id:int}/rechazar")]
        [HttpPut]
        public async Task<IActionResult> RechazarConsolidacionRequisitoExpedienteAsync(int id,
            RechazarConsolidacionRequisitoExpedienteCommand request)
        {
            request.Id = id;
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [Route("{id:int}/validar")]
        [HttpPut]
        public async Task<IActionResult> ValidarConsolidacionRequisitoExpedienteAsync(int id,
            ValidarConsolidacionRequisitoExpedienteCommand request)
        {
            request.Id = id;
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [Route("expediente/{idExpedienteAlumno:int}")]
        [HttpGet]
        public async Task<ActionResult<ConsolidacionRequisitoExpedienteDto[]>>
            GetConsolidacionesRequisitosByIdExpedienteAsync(int idExpedienteAlumno)
        {
            var result = await Mediator.Send(new GetConsolidacionesRequisitosByIdExpedienteQuery(idExpedienteAlumno));
            return Ok(result);
        }
    }
}
