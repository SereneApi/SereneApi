﻿using SereneApi.Core.Connection;
using SereneApi.Core.Content;
using SereneApi.Core.Requests;
using SereneApi.Core.Versioning;
using System;
using System.Collections.Generic;

namespace SereneApi.Handlers.Rest.Requests.Types
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
        public Method Method { get; set; }
        public object[] Parameters { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public string Resource { get; set; }
        public string ResourcePath { get; set; }
        public Type ResponseType { get; set; }
        public Uri Route { get; set; }
        public IApiVersion Version { get; set; }

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