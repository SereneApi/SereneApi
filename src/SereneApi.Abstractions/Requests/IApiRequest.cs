using SereneApi.Abstractions.Content;
using System;
using System.Collections.Generic;

namespace SereneApi.Abstractions.Requests
{
    /// <summary>
    /// A request to be performed against an API.
    /// </summary>
    public interface IApiRequest
    {
        /// <summary>
        /// The unique identity of the request.
        /// </summary>
        Guid Identity { get; }

        Uri Route { get; }

        /// <summary>
        /// The resource the request will act upon.
        /// </summary>
        /// <remarks>This is applied first[Resource/Version/Endpoint]</remarks>
        string Resource { get; }

        string ResourcePath { get; }

        /// <summary>
        /// The version the request will act upon.
        /// </summary>
        /// <remarks>This is applied after the Resource [Resource/Version/Endpoint]</remarks>
        IApiVersion Version { get; }

        /// <summary>
        /// The address used to make the request. This is applied after the source.
        /// </summary>
        /// <remarks>This is applied after the Version [Resource/Version/Endpoint]</remarks>
        string Endpoint { get; set; }

        public string EndpointTemplate { get; set; }

        /// <summary>
        /// The parameters used in the request.
        /// </summary>
        object[] Parameters { get; set; }

        Dictionary<string, string> Query { get; set; }

        /// <summary>
        /// The <see cref="Method"/> used when performing the request.
        /// </summary>
        Method Method { get; }

        /// <summary>
        /// The content contained within the body of the request.
        /// </summary>
        IRequestContent Content { get; }
    }
}
