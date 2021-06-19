using SereneApi.Abstractions;
using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Requests;
using System;
using System.Collections.Generic;

namespace SereneApi.Requests.Types
{
    public class ApiRequest : IApiRequest
    {
        public Guid Identity { get; } = Guid.NewGuid();
        public Uri Route { get; set; }
        public string Resource { get; set; }
        public string ResourcePath { get; set; }
        public IApiVersion Version { get; set; }
        public string Endpoint { get; set; }
        public string EndpointTemplate { get; set; }
        public object[] Parameters { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public Method Method { get; set; }
        public IRequestContent Content { get; set; }

        public Type ContentType { get; set; }

        public Type ResponseType { get; set; }
        /// <summary>
        /// An empty API request.
        /// </summary>
        /// <remarks>Used internally for Unit Testing.</remarks>
        public static IApiRequest Empty { get; } = new ApiRequest();
    }
}
