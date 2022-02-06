using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses;
using SereneApi.Extensions.Mocking.Rest.Settings;

namespace SereneApi.Extensions.Mocking.Rest.Responses
{
    public interface IMockResponse
    {
        public IRequestContent Content { get; }
        public DelaySettings Delay { get; }

        public Status Status { get; }
    }
}