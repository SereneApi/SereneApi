using SereneApi.Abstractions;
using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Requests;
using SereneApi.Abstractions.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SereneApi.Adapters.Profiling.Request
{
    [DebuggerDisplay("{Method} | {Endpoint}")]
    internal class RequestProfile : IRequestProfile
    {
        public Type Source { get; }
        public Guid Identity { get; }
        public Uri Route { get; }
        public string Resource { get; }
        public IApiVersion Version { get; }
        public string Endpoint { get; set; }
        public string EndpointTemplate { get; set; }
        public object[] Parameters { get; set; }
        public Dictionary<string, string> Query { get; set; }
        public Method Method { get; }

        public IRequestContent Content { get; }

        public IApiResponse Response { get; set; }

        public int RetryAttempts { get; set; } = 0;

        public DateTimeOffset Sent { get; set; }

        public DateTimeOffset Received { get; set; }

        public TimeSpan RequestDuration => Received.Subtract(Sent);

        public string ResourcePath { get; set; }

        public RequestProfile(IApiRequest request, Type sourceApi)
        {
            Identity = request.Identity;
            Endpoint = request.Endpoint;
            EndpointTemplate = request.EndpointTemplate;
            Parameters = request.Parameters;
            Query = request.Query;
            Route = request.Route;
            Resource = request.Resource;
            ResourcePath = request.ResourcePath;
            Version = request.Version;
            Method = request.Method;
            Content = request.Content;

            Source = sourceApi;
        }
    }
}
