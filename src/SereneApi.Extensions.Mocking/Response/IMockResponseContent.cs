using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Mocking.Response
{
    public interface IMockResponseContent
    {
        /// <summary>
        /// The <see cref="IMockResponse"/> only responds to the requests that have the specified in body content.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> of object to be responded to.</typeparam>
        /// <param name="inBodyContent">The object the <see cref="IMockResponse"/> will respond to.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        void RespondsToRequestsWith<TContent>([NotNull] TContent inBodyContent);
    }
}
