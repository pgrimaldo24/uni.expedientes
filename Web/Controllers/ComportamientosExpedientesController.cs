using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateNivelUsoComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateRequisitoComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientosExpedientesMasivo;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteNivelUsoComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitoComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitosComportamientosExpedientesMasivo;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.EditRequisitoComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetNivelUsoDisplayNameComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetPagedComportamientosExpedientes;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetTiposNivelesUso;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;
using Unir.Framework.WebApiSuperTypes.Models.Cal;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/comportamientos-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ComportamientosExpedientesController : ControllerSuperType
    {
        [Route("query")]
        [HttpPost]
        public async Task<ActionResult<PagedListResponseMessage<ComportamientosExpedientesListItemDto>>> GetRequisitosExpedientesAsync(
            ComportamientosExpedientesCriteriaRequest criteria)
        {
            var result = await Mediator.Send(criteria.GetQueryParameters());
            var response = new PagedListResponseMessage<ComportamientosExpedientesListItemDto>
            {
                Data = result.Elements,
                Meta = new MetaResponse
                {
                    TotalCount = result.TotalElements
                }
            };
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteComportamientoAsync(int id)
        {
            await Mediator.Send(new DeleteComportamientoExpedienteCommand(id));
            return NoContent();
        }

        [HttpPost("delete-massive")]
        public async Task<ActionResult> DeleteComportamientosMasivoAsync(
            DeleteComportamientosExpedientesMasivoCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<ActionResult<ComportamientoExpedienteDto>> GetComportamientoExpedienteAsync(int id)
        {
            var result = await Mediator.Send(new GetComportamientoExpedienteByIdQuery(id));
            return Ok(result);
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult> CreateComportamientoExpedienteAsync(CreateComportamientoExpedienteCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> EditComportamientoAsync(int id, EditComportamientoExpedienteCommand request)
        {
            request.IdComportamiento = id;
            await Mediator.Send(request);
            return NoContent();
        }

        [HttpPut("requisito-comportamiento/{id:int}")]
        public async Task<ActionResult> EditRequisitoComportamientoAsync(int id, 
            EditRequisitoComportamientoExpedienteCommand request)
        {
            request.IdRequisitoComportamiento = id;
            await Mediator.Send(request);
            return NoContent();
        }

        [HttpPost("{id:int}/requisito-comportamiento")]
        public async Task<ActionResult> CreateRequisitoComportamientoAsync(int id,
            CreateRequisitoComportamientoExpedienteCommand request)
        {
            request.IdComportamiento = id;
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("{id:int}/nivel-uso-comportamiento")]
        public async Task<ActionResult> CreateNivelUsoComportamientoAsync(int id,
            CreateNivelUsoComportamientoExpedienteCommand request)
        {
            request.IdComportamiento = id;
            var result = await Mediator.Send(request);
            return Ok(result);
        }


        [Route("tipos-niveles-uso/query")]
        [HttpPost]
        public async Task<ActionResult<TipoNivelUsoListItemDto[]>> GetTiposNivelesUsoAsync(
        TiposNivelesUsoCriteriaRequest criteria)
        {
            criteria.Offset = criteria.Limit * (criteria.Offset - 1);
            var result = await Mediator.Send(criteria.GetQueryParameters());
            return Ok(result);
        }

        [HttpDelete("{id:int}/requisito-comportamiento")]
        public async Task<ActionResult> DeleteRequisitoComportamientoAsync(int id)
        {
            await Mediator.Send(new DeleteRequisitoComportamientoExpedienteCommand(id));
            return NoContent();
        }

        [HttpPost("requisito-comportamiento-massive")]
        public async Task<ActionResult> DeleteRequisitosComportamientosMasivoAsync(
            DeleteRequisitosComportamientosExpedientesMasivoCommand request)
        {
            await Mediator.Send(request);
            return NoContent();
        }

        [HttpDelete("{id:int}/nivel-uso-comportamiento")]
        public async Task<ActionResult> DeleteNivelUsoComportamientoAsync(int id)
        {
            await Mediator.Send(new DeleteNivelUsoComportamientoExpedienteCommand(id));
            return NoContent();
        }

        [HttpPost("nivel-uso-comportamiento/display-name")]
        public async Task<ActionResult> GetNivelUsoDisplayNameComportamientoAsync(
            GetNivelUsoDisplayNameComportamientoExpedienteQuery request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }
    }
}
