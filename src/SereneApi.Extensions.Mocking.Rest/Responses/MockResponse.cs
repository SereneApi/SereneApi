using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses;
using SereneApi.Extensions.Mocking.Rest.Settings;

namespace SereneApi.Extensions.Mocking.Rest.Responses
{
    internal class MockResponse : IMockResponse
    {
        public IRequestContent Content { get; set; }
        public DelaySettings Delay { get; set; }

        public Status Status { get; set; }
    }
}