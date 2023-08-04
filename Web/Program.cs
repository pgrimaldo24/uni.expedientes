using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Unir.Expedientes.Persistence.Context;
using Unir.Framework.PersistenceSuperTypes;
using Unir.Framework.WebApiSuperTypes.Extensions;
using Unir.Framework.WebApiSuperTypes.Providers.Migrations;

namespace Unir.Expedientes.WebUi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var migrationCommandOption = MigrationsCommandOption.GetAppInstance(args);

            if (migrationCommandOption != null)
            {
                var host = GetHostBuilder(Array.Empty<string>()).Build();
                ExecuteMigrations(host, migrationCommandOption);
            }
            else
            {
                var host = GetHostBuilder(args).Build();
                host.Run();
            }
        }

        private static IHostBuilder GetHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                        .ConfigureAppConfiguration((builder, config) =>
                        {
                            builder.HostingEnvironment.EnvironmentName = Environment.GetEnvironmentVariable("ENTORNO");
                            if (!builder.HostingEnvironment.IsUnirValid())
                            {
                                throw new InvalidOperationException("El valor de la variable de entorno: ENTORNO no es válido. Especifique uno de los siguientes: local, des, lab, pre, pro");
                            }
                            config.LoadConfigurationFromDefaultFiles(builder.HostingEnvironment.EnvironmentName);
                        })
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseStartup<Startup>();
                });

        private static void ExecuteMigrations(IHost host, MigrationsCommandOption migrationCommandOption)
        {
            using var scope = host.Services.CreateScope();

            // Obtener instancia del DbContext
            var context = scope.ServiceProvider.GetService<ExpedientesContext>();
            if (context == null)
            {
                throw new InvalidOperationException("Contexto de Datos no registrado.");
            }

            // Registrar en el Log los detalles de la carga de configuración
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            var env = host.Services.GetRequiredService<IWebHostEnvironment>();
            if (logger != null && env != null)
            {
                logger.LogInformation(env.GetConfigurationLoadOrderDetails());
            }

            // Ejecutar migración según opción seleccionada en línea de comandos
            if (migrationCommandOption.CheckMigration)
            {
                context.ExecuteMigration(new MigrationAction(MigrationType.Check, null));
            }
            else if (migrationCommandOption.Target != null)
            {
                context.ExecuteMigration(new MigrationAction(MigrationType.UpdateTo, migrationCommandOption.Target));
            }
            else
            {
                context.ExecuteMigration(new MigrationAction(MigrationType.Update, null));
            }
        }
    }
}
