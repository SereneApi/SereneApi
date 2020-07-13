using SereneApi.Abstractions.Request.Content;
using System;

namespace SereneApi.Abstractions.Request
{
    /// <summary>
    /// A request to be performed against an API.
    /// </summary>
    public interface IApiRequest
    {
        /// <summary>
        /// The address used to make the request. This is applied after the source.
        /// </summary>
        Uri Endpoint { get; }

        /// <summary>
        /// The <see cref="Method"/> used when performing the request.
        /// </summary>
        Method Method { get; }

        /// <summary>
        /// The content contained within the body of the request.
        /// </summary>
        IApiRequestContent Content { get; }
    }
}
