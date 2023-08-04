using Microsoft.AspNetCore.Mvc;
using System;

namespace Unir.Expedientes.WebUi.Middleware.BasicAuth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class BasicAuthAttribute : TypeFilterAttribute
    {
        public BasicAuthAttribute(string realm = @"Application") : base(typeof(BasicAuthFilter))
        {
            Arguments = new object[] { realm };
        }
    }
}
