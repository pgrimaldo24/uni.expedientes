using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unir.Expedientes.Application.Tests.Common
{
    public class ServicesResolverForTest
    {
        public static void SetPrivateField(object serviceInstance, string fieldName, object value)
        {
            var fieldInfo = serviceInstance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                return;

            fieldInfo.SetValue(serviceInstance, value);
        }

        public static object InvokePrivateMethod(object serviceInstance, string methodName, params object[] arguments)
        {
            var methodInfo = serviceInstance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                throw new Exception($"Nombre de método: '{methodName}' incorrecto.");
            }

            return methodInfo.Invoke(serviceInstance, arguments);
        }

        public static Mock<T> GetMock<T>(params object[] args) where T : class
        {
            var parameters = typeof(T).GetConstructors()
                .First().GetParameters()
                .OrderBy(a => a.Position).ToList();
            var argumentosContructor = new List<object>();
            args ??= new object[0];
            foreach (var parameter in parameters)
            {
                var argument = args.FirstOrDefault(arg =>
                {
                    if (arg == null) return false;

                    var type = arg.GetType();
                    var interfaces = type.GetInterfaces();
                    var success = interfaces.Any(i => i.Name == parameter.ParameterType.Name) ||
                                  type.Name == parameter.ParameterType.Name ||
                                  type.BaseType?.Name == parameter.ParameterType.Name;
                    return success;
                });
                argumentosContructor.Add(argument);
            }
            var mockSut = new Mock<T>(argumentosContructor.ToArray())
            {
                CallBase = true
            };
            return mockSut;
        }

        public static Mock<T> GetMock<T>(bool callBase = true, params object[] args) where T : class
        {
            var parameters = typeof(T).GetConstructors()
                .First().GetParameters()
                .OrderBy(a => a.Position).ToList();
            var argumentosContructor = new List<object>();
            args ??= new object[0];
            foreach (var parameter in parameters)
            {
                var argument = args.FirstOrDefault(arg =>
                {
                    if (arg == null) return false;

                    var type = arg.GetType();
                    var interfaces = type.GetInterfaces();
                    var success = interfaces.Any(i => i.Name == parameter.ParameterType.Name) ||
                                  type.Name == parameter.ParameterType.Name ||
                                  type.BaseType?.Name == parameter.ParameterType.Name;
                    return success;
                });
                argumentosContructor.Add(argument);
            }
            var mockSut = new Mock<T>(argumentosContructor.ToArray())
            {
                CallBase = callBase
            };
            return mockSut;
        }

        public static T CreateInstance<T>(params object[] args) where T : class
        {
            var parameters = typeof(T).GetConstructors()
                .First().GetParameters()
                .OrderBy(a => a.Position).ToList();
            var argumentosContructor = new List<object>();
            args ??= new object[0];
            foreach (var parameter in parameters)
            {
                var argument = args.FirstOrDefault(arg =>
                {
                    if (arg == null) return false;

                    var type = arg.GetType();
                    var interfaces = type.GetInterfaces();
                    var success = interfaces.Any(i => i.Name == parameter.ParameterType.Name) ||
                                  type.Name == parameter.ParameterType.Name ||
                                  type.BaseType?.Name == parameter.ParameterType.Name;
                    return success;
                });
                argumentosContructor.Add(argument);
            }
            var newInstance = (T)Activator.CreateInstance(typeof(T), argumentosContructor.ToArray());
            return newInstance;
        }
    }
}
