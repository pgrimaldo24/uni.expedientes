using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/about")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class AboutController : ControllerSuperType
    {
        [Route("version")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Version()
        {
            var assembly = GetAssembly();
            var assemblyInfo = assembly.GetName();
            var version = assemblyInfo.Version?.ToString() ?? "";
            version = version[..version.LastIndexOf(".", StringComparison.Ordinal)];
            return Ok(await Task.Run(() => version));
        }

        private static Assembly GetAssembly()
        {
            return typeof(AboutController).Assembly;
        }
    }
}
