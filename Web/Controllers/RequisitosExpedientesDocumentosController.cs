using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.CreateRequisitoExpedienteDocumento;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitoExpedienteDocumento;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitosExpedientesDocumentos;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.EditRequisitoExpedienteDocumento;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Queries.GetAllRequisitosExpedientesDocumentosByIdRequisito;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/requisito-expediente-documento")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    [ApiController]
    public class RequisitosExpedientesDocumentosController : ControllerSuperType
    {
        [HttpPost]
        public async Task<ActionResult> CreateRequisitoExpedienteDocumentoAsync(CreateRequisitoExpedienteDocumentoCommand request)
        {
            await Mediator.Send(request);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> EditRequisitoExpedienteDocumentoAsync(EditRequisitoExpedienteDocumentoCommand request)
        {
            await Mediator.Send(request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRequisitoExpedienteDocumentoAsync(int id)
        {
            await Mediator.Send(new DeleteRequisitoExpedienteDocumentoCommand(id));
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetRequisitosExpedientesDocumentosByIdRequisitoAsync(int id)
        {
            return Ok(await Mediator.Send(new GetAllRequisitosExpedientesDocumentosByIdRequisitoQuery(id)));
        }

        [HttpPost("delete-massive")]
        public async Task<ActionResult> DeleteRequisitosExpedientesDocumentosMasivoAsync(
            DeleteRequisitosExpedientesDocumentosCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }
    }
}
