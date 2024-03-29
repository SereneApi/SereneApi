﻿using SereneApi.Core.Http;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Versioning;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Handlers.Rest.Requests
{
    public class RestApiRequest : IRestApiRequest
    {
        /// <summary>
        /// An empty API request.
        /// </summary>
        /// <remarks>Used internally for Unit Testing.</remarks>
        public static IApiRequest Empty { get; } = new RestApiRequest();

        public IRequestContent Content { get; set; }
        public Type ContentType { get; set; }
        public string Endpoint { get; set; }
        public string EndpointTemplate { get; set; }
        public Guid Identity { get; } = Guid.NewGuid();
        public HttpMethod HttpMethod { get; set; }
        public object[] Parameters { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public string Resource { get; set; }
        public string ResourcePath { get; set; }
        public Type ResponseType { get; set; }
        public Uri Route { get; set; }
        public Uri Url { get; set; }
        public IApiVersion Version { get; set; }
        public IReadOnlyDictionary<string, object> Headers { get; set; }

        public static RestApiRequest Create(IConnectionSettings connection)
        {
            RestApiRequest apiRequest = new RestApiRequest
            {
                Resource = connection.Resource,
                ResourcePath = connection.ResourcePath
            };

            return apiRequest;
        }
    }
}