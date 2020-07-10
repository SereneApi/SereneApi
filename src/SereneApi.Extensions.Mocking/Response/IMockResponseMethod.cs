using SereneApi.Abstractions.Request;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Mocking.Response
{
    public interface IMockResponseMethod: IMockResponseUrl
    {
        /// <summary>
        /// The <see cref="IMockResponse"/> only responds to requests that are of the specified <see cref="Method"/>.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> this <see cref="IMockResponse"/> will respond to.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty or an invalid <see cref="Method"/> is provided.</exception>
        IMockResponseUrl RespondsToRequestsWith([NotNull] params Method[] method);
    }
}
