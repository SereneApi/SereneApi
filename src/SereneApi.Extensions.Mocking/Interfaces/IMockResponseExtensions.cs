using SereneApi.Abstractions.Requests;
using SereneApi.Extensions.Mocking.Types;
using System;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IMockResponseExtensions
    {
        /// <summary>
        /// The <see cref="IMockResponse"/> only responds to requests within the specified <see cref="Uri"/>s.
        /// </summary>
        /// <param name="uris">An array of <see cref="Uri"/>s that this <see cref="MockResponse"/> will respond to.</param>
        /// <exception cref="ArgumentException">Thrown if no uris are supplied.</exception>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown if this method is called twice.</exception>
        IMockResponseExtensions RespondsToRequestsWith(params string[] uris);

        /// <summary>
        /// The <see cref="IMockResponse"/> only responds to the requests that have the specified in body content.
        /// </summary>
        /// <typeparam name="TContent">The <see cref="Type"/> of object to be responded to.</typeparam>
        /// <param name="inBodyContent">The object the <see cref="IMockResponse"/> will respond to.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        IMockResponseExtensions RespondsToRequestsWith<TContent>(TContent inBodyContent);

        /// <summary>
        /// The <see cref="IMockResponse"/> only responds to requests that are of the specified <see cref="Method"/>.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> this <see cref="IMockResponse"/> will respond to.</param>
        /// <exception cref="ArgumentException">Thrown if an invalid <see cref="Method"/> is provided.</exception>
        IMockResponseExtensions RespondsToRequestsWith(Method method);

        /// <summary>
        /// Delays the <see cref="IMockResponse"/> by the specified number of seconds.
        /// </summary>
        /// <remarks>This can be useful for testing timeouts and latency.</remarks>
        /// <param name="seconds">The number of seconds the response will be delayed by.</param>
        /// <exception cref="MethodAccessException">Thrown if this method has already been called.</exception>
        /// <exception cref="ArgumentException">Thrown if an invalid value is provided.</exception>
        IMockResponseExtensions ResponseIsDelayed(int seconds, int delayCount = 0);
    }
}
