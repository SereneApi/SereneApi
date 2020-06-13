using SereneApi.Extensions.Mocking.Types;
using System;

namespace SereneApi.Extensions.Mocking.Interfaces
{
    public interface IMockResponseExtensions
    {
        /// <summary>
        /// The <see cref="MockResponse"/> will only apply to requests with the specified <see cref="Uri"/>s.
        /// </summary>
        /// <param name="uris">An array of <see cref="Uri"/>s that this <see cref="MockResponse"/> will apply to.</param>
        IMockResponseExtensions RespondsToRequestsWith(params string[] uris);

        /// <summary>
        /// The <see cref="MockResponse"/> will only apply to the requests that have the specified in body content.
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="inBodyContent"></param>
        IMockResponseExtensions RespondsToRequestsWith<TContent>(TContent inBodyContent);

        IMockResponseExtensions RespondsToRequestsWith(Method method);

        /// <summary>
        /// Delays the response by the specified number of seconds. This can be useful for testing timeouts.
        /// </summary>
        /// <param name="seconds">The number of seconds the response will be delayed by.</param>
        IMockResponseExtensions ResponseIsDelayed(int seconds, int delayCount = 0);
    }
}
