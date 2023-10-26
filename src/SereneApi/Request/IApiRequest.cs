using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Request
{
    /// <summary>
    /// A request to be performed against an API.
    /// </summary>
    public interface IApiRequest
    {
        /// <summary>
        /// The content contained within the body of the request.
        /// </summary>
        IRequestContent Content { get; }

        /// <summary>
        /// The unique identity of the request.
        /// </summary>
        Guid Identity { get; }

        /// <summary>
        /// The <see cref="HttpMethod"/> used when performing the request.
        /// </summary>
        HttpMethod HttpMethod { get; }

        /// <summary>
        /// The resource the request will act upon.
        /// </summary>
        /// <remarks>This is applied first[Resource/Version/Endpoint]</remarks>
        string Resource { get; }

        string ResourcePath { get; }

        /// <summary>
        /// The address used to make the request. This is applied after the source.
        /// </summary>
        /// <remarks>This is applied after the Version [Resource/Version/Endpoint]</remarks>
        //string Endpoint { get; set; }
        public Type ResponseType { get; }

        Uri Route { get; }

        public Uri Url { get; }

        /// <summary>
        /// The version the request will act upon.
        /// </summary>
        /// <remarks>This is applied after the Resource [Resource/Version/Endpoint]</remarks>
        IApiVersion Version { get; }

        IReadOnlyDictionary<string, object> Headers { get; }
    }
}
