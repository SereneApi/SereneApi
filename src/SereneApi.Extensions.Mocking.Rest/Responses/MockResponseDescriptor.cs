using SereneApi.Core.Content;
using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Responses
{
    internal class MockResponseDescriptor : IMockResponseDescriptor
    {
        public IRequestContent Content { get; set; }
        public Uri[] Endpoints { get; set; }
        public Method[] Methods { get; set; }
        public IMockResponse Response { get; set; }
    }
}