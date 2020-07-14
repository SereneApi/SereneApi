using DeltaWare.Dependencies;
using System;

namespace SereneApi.Abstractions.Options
{
    public static class ApiOptionsExtensionsExtension
    {
        /// <summary>
        /// Gets the <see cref="IDependencyCollection"/> from <see cref="IApiOptionsExtensions"/>.
        /// </summary>
        /// <remarks>Must inherit <see cref="ICoreOptions"/>.</remarks>
        /// <exception cref="InvalidCastException">Thrown if <see cref="IApiOptionsExtensions"/> does not implement <see cref="ICoreOptions"/>.</exception>
        public static IDependencyCollection GetDependencyCollection(this IApiOptionsExtensions extensions)
        {
            if(extensions is ICoreOptions options)
            {
                return options.Dependencies;
            }

            throw new InvalidCastException($"Must inherit from {nameof(ICoreOptions)}");
        }
    }
}
