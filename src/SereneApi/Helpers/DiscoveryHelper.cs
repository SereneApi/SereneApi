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

        public static IEnumerable<Type> GetConcreteTypesOf<T>()
            => GetConcreteTypesOf(typeof(T));

        public static IEnumerable<Type> GetConcreteTypesOf(Type concreteType)
            => AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetLoadedTypes())
                .Where(t => ConcreteTypePredicate(t, concreteType));

        private static bool InterfaceImplementsAttributePredicate(Type interfaceType, Type attributeType)
        {
            return interfaceType.GetCustomAttribute(attributeType) != null;
        }

        private static bool ConcreteTypePredicate(Type sourceType, Type concreteType)
        {
            if (sourceType.IsAbstract)
            {
                return false;
            }

            if (concreteType.IsAssignableFrom(sourceType))
            {
                return true;
            }

            if (!concreteType.IsGenericType)
            {
                return false;
            }

            return sourceType.GetInterfaces().Any(t => t.IsGenericType && concreteType.IsAssignableFrom(t.GetGenericTypeDefinition()));
        }

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
