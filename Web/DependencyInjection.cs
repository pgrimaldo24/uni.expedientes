using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.WebApiSuperTypes.Auth;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Extensions;

namespace Unir.Expedientes.WebUi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<IExpedientesContext>());
            services.AddCors(o => o.AddPolicy(Startup.CORS_POLICY, builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddOpenApiDocument(cfg =>
            {
                cfg.DocumentProcessors.Add(new SecurityDefinitionAppender("ApiKey", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "X-Api-Key",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "El ApiKey"
                }));
                cfg.OperationProcessors.Add(new OperationSecurityScopeProcessor("ApiKey"));
            });

            // Configurar Forwarded Host y Schema
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddHttpContextAccessor();
            services.AddTransient<IIdentityService, HttpIdentityService>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = OidcAuthenticationOptions.DEFAULT_SCHEME;
                    options.DefaultChallengeScheme = OidcAuthenticationOptions.DEFAULT_SCHEME;
                })
                .AddApiKeySupport(options => { })
                .AddOidcBearerTokenSupport(options => { });

            // Servir ficheros estáticos de capa de presentación
            services.AddSpaStaticFiles(options =>
            {
                options.RootPath = "ClientApp/dist";
            });

            return services;
        }
    }
}
