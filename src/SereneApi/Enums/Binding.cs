using SereneApi.Interfaces;
using System;

namespace SereneApi.Enums
{
    /// <summary>
    /// <see cref="Binding"/> controls how <see cref="IDisposable"/>s will be handled when a <see cref="IDependency"/> is disposed of
    /// </summary>
    public enum Binding
    {
        /// <summary>
        /// The objects lifetime is bound to the <see cref="IDependency"/> and will be disposed of
        /// </summary>
        Bound,
        /// <summary>
        /// The objects lifetime is unbound from the <see cref="IDependency"/> and will not be disposed of
        /// </summary>
        Unbound
    }
}
