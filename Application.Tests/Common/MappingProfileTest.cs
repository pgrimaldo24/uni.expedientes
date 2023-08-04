using AutoMapper;
using System;
using System.Linq;
using System.Reflection;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.Tests.Common
{
    public class MappingProfileTest : Profile
    {
        public MappingProfileTest()
        {
            ApplyMappingsFromAssembly(typeof(DependencyInjection).Assembly);
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping");

                if (methodInfo == null)
                {
                    var interfaces = type.GetInterfaces();
                    var interfaceType = interfaces.FirstOrDefault(
                        t => t.GetGenericTypeDefinition() == typeof(IMapFrom<>).GetGenericTypeDefinition());
                    if (interfaceType != null)
                    {
                        methodInfo = interfaceType.GetMethod("Mapping");
                    }
                }

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}
