using SereneApi.Core.Http.Content;
using SereneApi.Extensions.Mocking.Rest.Responses;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal interface IMockResponseDescriptor : IMockRequestDescriptor
    {
        public IRequestContent Content { get; }
        public IMockResponse Response { get; }
    }
}
