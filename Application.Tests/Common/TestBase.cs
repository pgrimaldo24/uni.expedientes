using System;
using System.Globalization;
using System.Threading;
using Unir.Expedientes.Persistence.Context;

namespace Unir.Expedientes.Application.Tests.Common
{
    public class TestBase : IDisposable
    {
        protected readonly ExpedientesContext Context;

        public TestBase()
        {
            Context = DemoContextFactory.Create();
            var ci = new CultureInfo("es");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        public void Dispose()
            {
            DemoContextFactory.Destroy(Context);
        }
    }
}
