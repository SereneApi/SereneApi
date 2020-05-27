using DeltaWare.SereneApi.Interfaces;
using System;

namespace DeltaWare.SereneApi.Types
{
    internal static class IDependencyExtensions
    {
        public static TDependency GetInstance<TDependency>(this IDependency dependency)
        {
            if (dependency is Dependency<TDependency> dependencyValue && dependencyValue.Type == typeof(TDependency))
            {
                return dependencyValue.Instance;
            }

            throw new ArgumentException("Invalid Dependency");
        }
    }
}
