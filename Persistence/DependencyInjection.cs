using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Persistence.Context;

namespace Unir.Expedientes.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ExpedientesContext>(o =>
                o.UseSqlServer(configuration["DBConnectionString"]));

            // Configuración de la Aplicación
            services.AddTransient<AppConfiguration>();

            services.AddScoped<IExpedientesContext>(provider => provider.GetService<ExpedientesContext>());

            return services;
        }
    }
}
