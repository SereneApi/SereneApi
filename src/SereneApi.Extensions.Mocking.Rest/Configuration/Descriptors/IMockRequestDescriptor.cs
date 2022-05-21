using SereneApi.Core.Requests;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal interface IMockRequestDescriptor
    {
        public string[] Endpoints { get; }
        public Method[] Methods { get; }
    }
}