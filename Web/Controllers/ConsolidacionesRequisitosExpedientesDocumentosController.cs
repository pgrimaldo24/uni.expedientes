using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.DeleteConsolidacionRequisitoDocumentosById;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UpdateConsolidacionRequisitoDocumentosById;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UploadFileConsolidacionRequisitoDocumento;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.DownloadFicheroConsolidacionRequisitoDocumentoById;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.GetConsolidacionRequisitoDocumentosByIdConsolidacion;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;


namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/consolidaciones-requisitos-expedientes-documentos")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ConsolidacionesRequisitosExpedientesDocumentosController : ControllerSuperType
    {
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteConsolidacionRequisitoDocumentosByIdAsync(int id)
        {
            var result = await Mediator.Send(new DeleteConsolidacionRequisitoDocumentosByIdCommand(id));
            return Ok(result);
        }

        [Route("{id:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateConsolidacionRequisitoDocumentosByIdAsync(int id,
            UpdateConsolidacionRequisitoDocumentosByIdCommand request)
        {
            request.Id = id;
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [Route("{id:int}/download")]
        [HttpGet]
        public async Task<IActionResult> DownloadFicheroConsolidacionRequisitoDocumentoAsync(int id)
        {
            var result = await Mediator.Send(new DownloadFicheroConsolidacionRequisitoDocumentoByIdQuery(id));
            if (result?.Content == null)
                return NoContent();
            Response.Headers.Add("X-File-Name", result.FileName);
            Response.Headers.Add("Access-Control-Expose-Headers", "X-File-Name");
            return File(new MemoryStream(result.Content), MediaTypeNames.Application.Pdf, result.FileName);
        }

        [Route("upload-file/consolidacion-requisito-expediente/{id:int}")]
        [HttpPost]
        public async Task<IActionResult> UploadFileConsolidacionRequisitoDocumentoAsync(int id,
            [FromForm] UploadFileConsolidacionRequisitoDocumentoCommand command)
        {
            command.IdConsolidacionRequisito = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [Route("consolidacion-requisito-expediente/{id:int}")]
        [HttpGet]
        public async Task<ActionResult<ConsolidacionRequisitoExpedienteDocumentoListItemDto[]>>
            GetConsolidacionRequisitoDocumentosByIdConsolidacionAsync(int id)
        {
            var result = await Mediator.Send(new GetConsolidacionRequisitoDocumentosByIdConsolidacionQuery(id));
            return Ok(result);
        }
    }
}
