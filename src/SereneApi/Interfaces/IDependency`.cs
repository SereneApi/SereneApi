using System;

namespace SereneApi.Interfaces
{
    public interface IDependency<out TDependency> : IDependency
    {
        /// <summary>
        /// The current instance of the <see cref="TDependency"/>
        /// </summary>
        public TDependency Instance { get; }

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="TDependency"/>
        /// </summary>
        Type Type { get; }
    }
}
