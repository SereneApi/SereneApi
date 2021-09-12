using SereneApi.Core.Content;
using SereneApi.Core.Responses;
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