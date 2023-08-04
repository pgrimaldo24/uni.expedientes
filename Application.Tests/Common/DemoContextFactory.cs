using Microsoft.EntityFrameworkCore;
using System;
using Unir.Expedientes.Persistence.Context;

namespace Unir.Expedientes.Application.Tests.Common
{
    public class DemoContextFactory
    {
        public static ExpedientesContext Create()
        {
            var options = new DbContextOptionsBuilder<ExpedientesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
                .Options;

            var context = new ExpedientesContext(options);

            context.Database.EnsureCreated();

            context.SaveChanges();

            return context;
        }

        public static void Destroy(ExpedientesContext context)
        {
            context.Database.EnsureDeleted();

            context.Dispose();
        }
    }
}
