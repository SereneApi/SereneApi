using System.Net.Http;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal interface IMockRequestDescriptor
    {
        public string[] Endpoints { get; }
        public HttpMethod[] Methods { get; }
    }
}