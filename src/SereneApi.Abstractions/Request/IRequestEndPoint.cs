using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Request
{
    public interface IRequestEndpoint: IRequestContent
    {
        /// <summary>
        /// Provides a single parameter which will used as the endpoint.
        /// </summary>
        /// <param name="parameter">The parameter to be used as the endpoint.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestContent WithParameter([NotNull] object parameter);
        /// <summary>
        /// Provides a string which will be used as the endpoint, this value can be a formattable string when used in conjunction with WithParameters"/>
        /// </summary>
        /// <param name="endpoint">The endpoint of the request</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        IRequestEndPointParams WithEndPoint([NotNull] string endpoint);
    }
}
