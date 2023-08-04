using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == typeof(IMapFrom<>) || i.GetGenericTypeDefinition() == typeof(IMapTo<>))))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping");

                if (methodInfo == null)
                {
                    var interfaces = type.GetInterfaces();

                    var interfaceMapFromType = interfaces.FirstOrDefault(
                        t => t.GetGenericTypeDefinition() == typeof(IMapFrom<>).GetGenericTypeDefinition());
                    var interfaceMapToType = interfaces.FirstOrDefault(
                        t => t.GetGenericTypeDefinition() == typeof(IMapTo<>).GetGenericTypeDefinition());

                    if (interfaceMapFromType != null)
                    {
                        methodInfo = interfaceMapFromType.GetMethod("Mapping");
                        methodInfo?.Invoke(instance, new object[] { this });
                    }
                    if (interfaceMapToType != null)
                    {
                        methodInfo = interfaceMapToType.GetMethod("Mapping");
                        methodInfo?.Invoke(instance, new object[] { this });
                    }
                }
                else
                {
                    methodInfo.Invoke(instance, new object[] {this});
                }
            }
        }
    }
}