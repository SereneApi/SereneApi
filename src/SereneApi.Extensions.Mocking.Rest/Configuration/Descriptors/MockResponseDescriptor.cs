using SereneApi.Core.Http.Content;
using SereneApi.Extensions.Mocking.Rest.Responses;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal class MockResponseDescriptor : IMockResponseDescriptor
    {
        public IRequestContent Content { get; set; }
        public string[] Endpoints { get; set; }
        public HttpMethod[] Methods { get; set; }
        public IMockResponse Response { get; set; }
    }
}