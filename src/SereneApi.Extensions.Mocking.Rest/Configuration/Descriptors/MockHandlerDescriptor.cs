using SereneApi.Extensions.Mocking.Rest.Handler.Attributes;
using System;
using System.Net.Http;
using System.Reflection;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal class MockHandlerDescriptor : IMockHandlerDescriptor
    {
        public string[] Endpoints { get; set; }
        public HttpMethod[] Methods { get; set; }
        public Type HandlerType { get; set; }
        public MethodInfo Method { get; set; }
        public MockMethodAttribute MockMethod { get; set; }
    }
}
