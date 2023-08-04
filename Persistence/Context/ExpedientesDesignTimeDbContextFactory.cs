using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Unir.Expedientes.Persistence.Context
{
    public class ExpedientesDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ExpedientesContext>
    {
        public ExpedientesContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Web"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ExpedientesContext>();
            optionsBuilder.UseSqlServer(config["DBConnectionString"]);

            return new ExpedientesContext(optionsBuilder.Options);
        }
    }
}
