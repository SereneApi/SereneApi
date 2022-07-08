using SereneApi.Core.Requests;
using SereneApi.Extensions.Mocking.Rest.Handler.Attributes;
using System;
using System.Reflection;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal class MockHandlerDescriptor : IMockHandlerDescriptor
    {
        public string[] Endpoints { get; set; }
        public Method[] Methods { get; set; }
        public Type HandlerType { get; set; }
        public MethodInfo Method { get; set; }
        public MockMethodAttribute MockMethod { get; set; }
    }
}
