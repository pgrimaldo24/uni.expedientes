using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Unir.Expedientes.Application;
using Unir.Expedientes.Application.Hub;
using Unir.Expedientes.Infrastructure;
using Unir.Expedientes.Persistence;
using Unir.Expedientes.WebUi.Subscriptions;
using Unir.Framework.Crosscutting.FileSystem;
using Unir.Framework.Crosscutting.MandrillMailSender;
using Unir.Framework.Crosscutting.OAuthSecurity;
using Unir.Framework.Crosscutting.QuartzJobs;
using Unir.Framework.Crosscutting.RabbitMqBus;
using Unir.Framework.Crosscutting.RedisCache;
using Unir.Framework.WebApiSuperTypes.Extensions;

namespace Unir.Expedientes.WebUi
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public const string CORS_POLICY = "AppCorsPolicy";
        public const string PATH_RESOURCES = "Resources/Globalization";

        // Carga de la configuración del sistema
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddLocalization();

            // Capas
            services.AddPresentation(_configuration);
            services.AddApplication(_configuration);
            services.AddPersistence(_configuration);
            services.AddInfrastructure(_configuration);

            // Componentes transversales
            services.AddRedisCache(_configuration);
            services.AddFileSystemManager(_configuration);
            services.AddOAuthSecurity(_configuration);
            services.AddSmtpEmail(_configuration);
            services.AddBackgorundJobs(_configuration);

            var rabbitMute = _env.IsUnirLocal();
            services.AddRabbitMqBus(rabbitMute, _configuration, new[] { typeof(MatriculaRealizadaSubscriptor),
                                                                   typeof(MatriculaDesestimadaSubscriptor),
                                                                   typeof(MatriculaReiniciadaSubscriptor),
                                                                   typeof(MatriculaRecuperadaSubscriptor),
                                                                   typeof(MatriculaAmpliacionRealizadaSubscriptor),
                                                                   typeof(MatriculaAnuladaSubscriptor),
                                                                   typeof(MatriculaAmpliacionDesestimadaSubscriptor),
                                                                   typeof(MatriculaAmpliacionReiniciadaSubscriptor),
                                                                   typeof(MatriculaAmpliacionRecuperadaSubscriptor),
                                                                   typeof(MatriculaVariacionDesestimadaSubscriptor),
                                                                   typeof(MatriculaVariacionRecuperadaSubscriptor),
                                                                   typeof(MatriculaAmpliacionAnuladaSubscriptor),
                                                                   typeof(MatriculaVariacionRealizadaSubscriptor),
                                                                   typeof(MatriculaVariacionReiniciadaSubscriptor),
                                                                   typeof(MatriculaVariacionAnuladaSubscriptor),
                                                                   typeof(NotaFinalGeneradaSubscriptor)
            });

            // Configuracion de la aplicación
            services.AddTransient<AppConfiguration>();

            // configuración de Globalización y Localización
            services.AddLocalization(options => options.ResourcesPath = PATH_RESOURCES);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger)
        {
            app.UseCors(CORS_POLICY);
            app.UseForwardedHeaders();
            app.UseStaticFiles();

            if (env.IsUnirLocal() || env.IsUnirDes())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            if (!env.IsUnirLocal())
            {
                app.UseSpaStaticFiles();
            }

            loggerFactory.AddLog4Net($"log4net.{env.EnvironmentName}.config");
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseRouting();
            app.UseCustomExceptionHandler();
            app.UseAuthentication();
            app.UseAuthorization();

            // Soporte para Globalización y Localización
            var languagesSuported = new[] { "es" };
            var localizaoptions = new RequestLocalizationOptions()
                .SetDefaultCulture(languagesSuported[0])
                .AddSupportedCultures(languagesSuported)
                .AddSupportedUICultures(languagesSuported);
            app.UseRequestLocalization(localizaoptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

            });

            logger.LogInformation(env.GetConfigurationLoadOrderDetails());
        }
    }
}
