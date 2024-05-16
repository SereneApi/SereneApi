using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SereneApi.Helpers
{
    internal static class DiscoveryHelper
    {
        public static IEnumerable<Type> GetInterfacesImplementingAttribute<T>()
            => GetInterfacesImplementingAttribute(typeof(T));

        public static IEnumerable<Type> GetInterfacesImplementingAttribute(Type attributeType)
            => AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetLoadedTypes())
                .Where(t => t.IsInterface)
                .Where(t => InterfaceImplementsAttributePredicate(t, attributeType));

        private static bool InterfaceImplementsAttributePredicate(Type interfaceType, Type attributeType)
            => interfaceType.GetCustomAttribute(attributeType) != null;

        private static Type[] GetLoadedTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types
                    .Where(t => t != null)
                    .ToArray();
            }
        }
    }
}
