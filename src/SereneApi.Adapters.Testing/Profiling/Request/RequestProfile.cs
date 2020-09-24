using System;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;

namespace SereneApi.Adapters.Testing.Profiling.Request
{
    internal class RequestProfile: IRequestProfile
    {
        public Type SourceApi { get; }

        public Guid Identity => Request.Identity;

        public TimeSpan RequestDuration { get; set; }

        public IApiRequest Request { get; }

        public IApiResponse Response { get; set; }

        public int RetryAttempts { get; set; }

        public DateTime Sent { get; set; }

        public DateTime Received { get; set; }

        public RequestProfile(IApiRequest request, Type sourceApi)
        {
            Request = request;
            SourceApi = sourceApi;
        }
    }
}
