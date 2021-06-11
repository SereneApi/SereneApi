using SereneApi.Abstractions.Requests;
using System;

namespace SereneApi.Requests
{
    public interface IApiRequestBuilder
    {
        /// <summary>
        /// Specifies the <see cref="Method"/> of the request.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> to be used.</param>
        /// <exception cref="ArgumentException">Thrown when an invalid <see cref="Method"/> is provided.</exception>
        IApiRequestResource UsingMethod(Method method);
    }
}
