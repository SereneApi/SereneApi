using System;

namespace SereneApi.Interfaces.Requests
{
    /// <summary>
    /// A request to be performed against an API.
    /// </summary>
    public interface IApiRequest
    {
        /// <summary>
        /// The endpoint used to make the request. This is applied after the source.
        /// </summary>
        Uri EndPoint { get; }

        /// <summary>
        /// The <see cref="Method"/> used when performing the request.
        /// </summary>
        Method Method { get; }

        /// <summary>
        /// The content contained in the body of the request.
        /// </summary>
        IApiRequestContent Content { get; }
    }
}
