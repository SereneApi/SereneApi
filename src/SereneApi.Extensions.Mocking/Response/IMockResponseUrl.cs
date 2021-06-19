using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Extensions.Mocking.Response
{
    public interface IMockResponseUrl : IMockResponseContent
    {
        /// <summary>
        /// The <see cref="IMockResponse"/> only responds to requests within the specified <see cref="Uri"/>s.
        /// </summary>
        /// <param name="uris">An array of <see cref="Uri"/>s that this <see cref="MockResponse"/> will respond to.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="ArgumentException">Thrown when the params are empty.</exception>
        IMockResponseContent RespondsToRequestsWith([NotNull] params string[] uris);
    }
}
