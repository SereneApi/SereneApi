using SereneApi.Core.Http;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Requests;
using SereneApi.Core.Versioning;
using System;

namespace SereneApi.Handlers.Soap.Requests.Types
{
    public class SoapApiRequest : ISoapApiRequest
    {
        /// <summary>
        /// An empty API request.
        /// </summary>
        /// <remarks>Used internally for Unit Testing.</remarks>
        public static IApiRequest Empty { get; } = new SoapApiRequest();

        public IRequestContent Content { get; set; }
        public Type ContentType { get; set; }
        public string Endpoint { get; set; }
        public Guid Identity { get; } = Guid.NewGuid();
        public Method Method { get; set; }
        public string Resource { get; set; }
        public string ResourcePath { get; set; }
        public Type ResponseType { get; set; }
        public Uri Route { get; set; }
        public IApiVersion Version { get; set; }

        public static SoapApiRequest Create(IConnectionSettings connection)
        {
            SoapApiRequest apiRequest = new SoapApiRequest
            {
                Resource = connection.Resource,
                ResourcePath = connection.ResourcePath
            };

            return apiRequest;
        }
    }
}