using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using System;
using System.Diagnostics;

namespace SereneApi.Adapters.Profiling.Request
{
    [DebuggerDisplay("{Method} | {Endpoint}")]
    internal class RequestProfile: IRequestProfile
    {
        public Type Source { get; }

        public Guid Identity { get; }

        public Uri Endpoint { get; }

        public Method Method { get; }

        public IApiRequestContent Content { get; }

        public IApiResponse Response { get; set; }

        public int RetryAttempts { get; set; } = 0;

        public DateTimeOffset Sent { get; set; }

        public DateTimeOffset Received { get; set; }

        public TimeSpan RequestDuration => Received.Subtract(Sent);

        public RequestProfile(IApiRequest request, Type sourceApi)
        {
            Identity = request.Identity;
            Endpoint = request.Endpoint;
            Method = request.Method;
            Content = request.Content;

            Source = sourceApi;
        }
    }
}
