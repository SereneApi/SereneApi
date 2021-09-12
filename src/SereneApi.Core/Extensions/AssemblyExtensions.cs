using System;
using System.Linq;
using System.Reflection;

namespace SereneApi.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static Type[] GetLoadedTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null).ToArray();
            }
        }
    }
}