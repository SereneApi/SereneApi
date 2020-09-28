using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Response;
using System;

namespace SereneApi.Adapters.Profiling.Profiling.Request
{
    internal class RequestProfile: IRequestProfile
    {
        public Type Source { get; }

        public Guid Identity { get; }

        public Uri Endpoint { get; }

        public Method Method { get; }

        public IApiRequestContent Content { get; }

        public IApiResponse Response { get; set; }

        public int RetryAttempts { get; set; } = 0;

        public DateTime Sent { get; set; }

        public DateTime Received { get; set; }

        public TimeSpan RequestDuration => Sent.Subtract(Received);

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
