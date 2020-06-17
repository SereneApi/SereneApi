using SereneApi.Enums;
using System;

namespace SereneApi.Interfaces
{
    public interface IDependency: ICloneable
    {
        /// <summary>
        /// The <see cref="Binding"/> of the <see cref="IDependency"/>.
        /// </summary>
        Binding Binding { get; }

        /// <summary>
        /// The current instance of the <see cref="IDependency"/>.
        /// </summary>
        object Instance { get; }

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="IDependency"/>,
        /// </summary>
        Type Type { get; }
    }
}
