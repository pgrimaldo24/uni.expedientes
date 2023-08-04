using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;

namespace Unir.Expedientes.WebUi
{
    public class CustomRequestCultureProvider : RequestCultureProvider
    {
        private readonly IConfiguration _configuration;
        private const string DefaultLanguage = "es";
        public CustomRequestCultureProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var language = _configuration["Language"] ?? DefaultLanguage;
            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            await Task.Yield();
            return new ProviderCultureResult(language);
        }
    }
}
