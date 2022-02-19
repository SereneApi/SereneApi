using SereneApi.Core.Http.Requests;
using System.Collections.Generic;

namespace SereneApi.Handlers.Rest.Requests
{
    public interface IRestApiRequest : IApiRequest
    {
        string Endpoint { get; set; }

        public string EndpointTemplate { get; set; }

        /// <summary>
        /// The parameters used in the request.
        /// </summary>
        object[] Parameters { get; set; }

        Dictionary<string, string> Query { get; set; }
    }
}