using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unir.Expedientes.Application.BackgroundJob;
using Unir.Expedientes.Application.BackgroundJob.MigrarCalificacionesExpedientes;
using Unir.Expedientes.Application.BackgroundJob.RelacionarExpedientesAlumnos;
using Unir.Expedientes.Application.BackgroundJob.SetEstadosHitosEspecializacionesExpedientesAlumnos;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Framework.ApplicationSuperTypes.Behaviours;

namespace Unir.Expedientes.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // Aplicacion
            services.AddAutoMapper(expression =>
            {
                expression.AddProfile(new MappingProfile(typeof(DependencyInjection).Assembly));
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Comportamientos de Commands
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            AppConfiguration.Enviroment = configuration.GetValue<string>("Enviroment");
            AppConfiguration.KeyAdminRole = configuration.GetValue<string>("Security:ApplicationRoles:ADMIN_ROLE");
            AppConfiguration.KeyGestorRole = configuration.GetValue<string>("Security:ApplicationRoles:GESTOR_ROLE");
            AppConfiguration.KeyAdminRoleName = configuration.GetValue<string>("Security:ApplicationRoles:ADMIN_ROLE_NAME");
            AppConfiguration.KeyGestorRoleName = configuration.GetValue<string>("Security:ApplicationRoles:GESTOR_ROLE_NAME");
            services.
                AddTransient<IBackgroundJobApplicationService, BackgroundJobApplicationService>();
            services
                .AddTransient<ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService,
                    SetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService>();
            services
                .AddTransient<IRelacionarExpedientesAlumnosApplicationService,
                    RelacionarExpedientesAlumnosApplicationService>();
            services
                .AddTransient<IMigrarCalificacionesExpedientesApplicationService,
                    MigrarCalificacionesExpedientesApplicationService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}
