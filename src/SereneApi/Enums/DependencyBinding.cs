using System;
using DeltaWare.SereneApi.Types;

namespace DeltaWare.SereneApi.Enums
{
    /// <summary>
    /// <see cref="DependencyBinding"/> controls how <see cref="IDisposable"/>s will be handled when a <see cref="Dependency"/> is disposed of
    /// </summary>
    public enum DependencyBinding
    {
        /// <summary>
        /// The objects lifetime is bound to the <see cref="Dependency"/> and will be disposed of
        /// </summary>
        Bound,
        /// <summary>
        /// The objects lifetime is unbound from the <see cref="Dependency"/> and will not be disposed of
        /// </summary>
        Unbound
    }
}
