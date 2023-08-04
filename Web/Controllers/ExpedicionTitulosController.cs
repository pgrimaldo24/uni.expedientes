using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTiposSolicitudes;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/integracion/expedicion-titulos")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ExpedicionTitulosController : ControllerSuperType
    {
        private readonly IExpedicionTitulosServiceClient _expedicionTitulosServiceClient;

        public ExpedicionTitulosController(IExpedicionTitulosServiceClient expedicionTitulosServiceClient)
        {
            _expedicionTitulosServiceClient = expedicionTitulosServiceClient;
        }

        [Route("estados-solicitudes")]
        [HttpGet]
        public async Task<ActionResult<List<SolicitudExpedicionTitulosModel>>> GetSolicitudes(
            int idIntegracionAlumno, int idPlan, bool excluirCanceladas)
        {
            var result = await _expedicionTitulosServiceClient.GetSolicitudes(
                                idIntegracionAlumno, idPlan, excluirCanceladas);
            return Ok(result);
        }

        [Route("tipos-solicitudes")]
        [HttpPost]
        public async Task<IActionResult> GetTiposSolicitudes(TiposSolicitudesParameters tiposSolicitudesParameters)
        {
            var result = await Mediator.Send(new GetTiposSolicitudesQuery { Nombre = tiposSolicitudesParameters.FilterDisplayName, IdsRefUniversidad = tiposSolicitudesParameters.IdsRefUniversidad });
            return Ok(result);
        }

        [Route("generar-solicitud-certificado")]
        [HttpPost]
        public async Task<IActionResult> GenerarSolicitudCertificado(GenerarSolicitudTituloCertificadoCommand request)
        {
            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [Route("generar-solicitud-certificado-masivo")]
        [HttpPost]
        public async Task<IActionResult> GenerarSolicitudCertificadoMasivo(GenerarSolicitudTituloCertificadoMasivoRequest criteria)
        {
            var request = criteria.GetQueryParameters();
            request.TipoSolicitud = criteria.TipoSolicitud;
            request.FechaSolicitud = criteria.FechaSolicitud;
            request.FechaPago = criteria.FechaPago;
            var result = await Mediator.Send(request);
            return Ok(result);
        }
    }
}
