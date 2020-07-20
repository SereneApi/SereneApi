using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Request
{
    public interface IRequestEndpoint: IRequestContent
    {
        /// <summary>
        /// Provides a parameter in which the request will be made against.
        /// </summary>
        /// <param name="parameter">The parameter to be used as the endpoint.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestContent WithEndpoint([NotNull] object parameter);
        /// <summary>
        /// Provides a parameter in which the request will be made against.
        /// </summary>
        /// <param name="endpoint">The endpoint of the request</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestContent WithEndpoint([NotNull] string endpoint);
        /// <summary>
        /// Provides a formatted endpoint template which the request will be made against.
        /// </summary>
        /// <param name="template">The endpoint template that will be formatted.</param>
        /// <param name="parameters">The parameters that will be appended to the template.</param>
        /// <exception cref="ArgumentException">Thrown when no parameters are provided.</exception>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestContent WithEndpointTemplate([NotNull] string template, [NotNull] params object[] parameters);
    }
}
