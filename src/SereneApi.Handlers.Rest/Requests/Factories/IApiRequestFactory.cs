using System;
using SereneApi.Core.Requests;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestFactory
    {
        /// <summary>
        /// Specifies the <see cref="Method"/> of the request.
        /// </summary>
        /// <param name="method">The <see cref="ArgumentException"/> to be used.</param>
        /// <exception cref="Method">Thrown when an invalid <see cref="Method"/> is provided.</exception>
        IApiRequestResource UsingMethod(Method method);
    }
}
