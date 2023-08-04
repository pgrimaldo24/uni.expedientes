using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasExpediente.Commands.CreateAsignaturaExpediente;
using Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetAsignaturasExpedientesByIdExpediente;
using Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetReconocimientos;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/asignaturas-expedientes")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    [ApiController]
    public class AsignaturasExpedienteController : ControllerSuperType
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsignaturaExpedienteAsync(
            CreateAsignaturaExpedienteCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("expediente/{idExpedienteAlumno:int}")]
        public async Task<IActionResult> GetAsignaturasByIdExpedienteAsync(
            int idExpedienteAlumno)
        {
            var result = await Mediator.Send(
                new GetAsignaturasExpedientesByIdExpedienteQuery(idExpedienteAlumno));
            return Ok(result);
        }

        [HttpGet]
        [Route("reconocimientos")]
        public async Task<IActionResult> GetReconocimientosAsync(string idIntegracionAlumno,
            string idRefPlan, string idRefVersionPlan)
        {
            var result = await Mediator.Send(
                new GetReconocimientosQuery(idIntegracionAlumno, idRefPlan, idRefVersionPlan));
            return Ok(result);
        }
    }
}
