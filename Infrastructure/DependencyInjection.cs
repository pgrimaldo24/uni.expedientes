using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Infrastructure.Fakes;
using Unir.Expedientes.Infrastructure.ServicesClients;
using Unir.Expedientes.Infrastructure.ServicesClients.Settings;

namespace Unir.Expedientes.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServicesClientsSettings>(configuration.GetSection("ServicesClients"));
            services.Configure<SecurityConfigurationModel>(configuration.GetSection("Security"));

            var settings = new ServicesClientsSettings();
            configuration.GetSection("ServicesClients").Bind(settings);

            services.AddHttpClient();
            services.AddTransient<ISecurityIntegrationServices, SecurityServiceClient>();
            services.AddTransient<IErpAcademicoServiceClient, ErpAcademicoServiceClient>();
            services.AddTransient<ICommonsServiceClient, CommonsServiceClient>();
            services.AddTransient<IExpedientesGestorUnirServiceClient, ExpedientesGestorUnirServiceClient>();
            services.AddTransient<IIntegrationFakeServices, IntegrationFakeServices>();
            services.AddTransient<IExpedicionTitulosServiceClient, ExpedicionTitulosServiceClient>();
            services.AddTransient<IFinancieroServiceClient, FinancieroServiceClient>();
            services.AddTransient<IGestorDocumentalServiceClient, GestorDocumentalServiceClient>();
            services.AddTransient<IEvaluacionesServiceClient, EvaluacionesServiceClient>();
            services.AddTransient<IGestorMapeosServiceClient, GestorMapeosServiceClient>();

            SetFakeImplementations(services, settings);

            return services;
        }

        private static void SetFakeImplementations(IServiceCollection services, ServicesClientsSettings settings)
        {
            if (settings.ErpAcademicoServices.Fake ?? false)
            {
                services.AddTransient<IErpAcademicoServiceClient, ErpAcademicoFakeServiceClient>();
            }

            if (settings.ExpedientesGestorUnirServices.Fake ?? false)
            {
                services.AddTransient<IExpedientesGestorUnirServiceClient, ExpedientesGestorUnirFakeServiceClient>();
            }

            if (settings.ExpedicionTitulosServices.Fake ?? false)
            {
                services.AddTransient<IExpedicionTitulosServiceClient, ExpedicionTitulosFakeServiceClient>();
            }

            if (settings.EvaluacionServices.Fake ?? false)
            {
                services.AddTransient<IEvaluacionesServiceClient, EvaluacionFakeServiceClient>();
            }

            if (settings.GestorMapeosServices.Fake ?? false)
            {
                services.AddTransient<IGestorMapeosServiceClient, GestorMapeosFakeServiceClient>();
            }
        }
    }
}
