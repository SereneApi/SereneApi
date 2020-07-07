using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Helpers;
using System;

namespace SereneApi.Types
{
    /// <inheritdoc cref="IApiRequest"/>
    public class ApiRequest: IApiRequest
    {
        /// <inheritdoc cref="IApiRequest.EndPoint"/>
        public Uri EndPoint { get; }

        /// <inheritdoc cref="IApiRequest.Method"/>
        public Method Method { get; }

        /// <inheritdoc cref="IApiRequest.Content"/>
        public IApiRequestContent Content { get; }

        /// <summary>
        /// Creates a new instance of a <see cref="ApiRequest"/>.
        /// </summary>
        /// <param name="method">The <see cref="Method"/> used when performing the request.</param>
        /// <param name="endPoint">The endpoint used to make the request. This is applied after the source.</param>
        /// <param name="content">The content contained in the body of the request.</param>
        /// <exception cref="ArgumentException">Thrown if an invalid method is provided.</exception>
        public ApiRequest(Method method, Uri endPoint = null, IApiRequestContent content = null)
        {
            ExceptionHelper.EnsureCorrectMethod(method);

            Method = method;
            EndPoint = endPoint;
            Content = content;
        }
    }

}
