using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses;
using SereneApi.Extensions.Mocking.Rest.Configuration.Settings;

namespace SereneApi.Extensions.Mocking.Rest.Responses
{
    internal interface IMockResponse
    {
        public IRequestContent Content { get; }
        public IDelaySettings Delay { get; }

        public Status Status { get; }
    }
}