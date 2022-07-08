using SereneApi.Core.Http.Content;
using SereneApi.Core.Requests;
using SereneApi.Extensions.Mocking.Rest.Responses;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal class MockResponseDescriptor : IMockResponseDescriptor
    {
        public IRequestContent Content { get; set; }
        public string[] Endpoints { get; set; }
        public Method[] Methods { get; set; }
        public IMockResponse Response { get; set; }
    }
}