using SereneApi.Core.Content;
using SereneApi.Core.Requests;
using SereneApi.Core.Versioning;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;

namespace SereneApi.Handlers.Soap.Requests.Types
{
    public class SoapApiRequest : ISoapApiRequest
    {
        public Guid Identity { get; } = Guid.NewGuid();
        public Uri Route { get; set; }
        public string Resource { get; set; }
        public string ResourcePath { get; set; }
        public IApiVersion Version { get; set; }
        public string Endpoint { get; set; }
        public Method Method { get; set; }
        public IRequestContent Content { get; set; }

        public Type ContentType { get; set; }

        public Type ResponseType { get; set; }

        public string Service { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// An empty API request.
        /// </summary>
        /// <remarks>Used internally for Unit Testing.</remarks>
        public static IApiRequest Empty { get; } = new SoapApiRequest();
    }
}
