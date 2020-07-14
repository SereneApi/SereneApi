using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Request
{
    public interface IRequest: IRequestEndpoint
    {
        /// <summary>
        /// The specific resource the request will be made against.
        /// </summary>
        /// <param name="resource">The API resource.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestEndpoint AgainstResource([NotNull] string resource);
    }
}
