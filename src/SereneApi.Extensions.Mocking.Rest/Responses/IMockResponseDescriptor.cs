using SereneApi.Core.Http.Content;
using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Responses
{
    public interface IMockResponseDescriptor
    {
        public IRequestContent Content { get; }
        public Uri[] Endpoints { get; }
        public Method[] Methods { get; }
        public IMockResponse Response { get; }
    }
}