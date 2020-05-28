using System;

namespace SereneApi.Interfaces
{
    public interface IApiHandlerOptions
    {
        /// <summary>
        /// The Dependencies required by the <see cref="ApiHandler"/>
        /// </summary>
        IDependencyCollection Dependencies { get; }

        /// <summary>
        /// The Source being used by the <see cref="ApiHandler"/>
        /// </summary>
        Uri Source { get; }
    }
}
