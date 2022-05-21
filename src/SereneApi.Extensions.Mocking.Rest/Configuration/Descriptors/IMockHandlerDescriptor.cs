using SereneApi.Extensions.Mocking.Rest.Handler.Attributes;
using System;
using System.Reflection;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors
{
    internal interface IMockHandlerDescriptor : IMockRequestDescriptor
    {
        Type HandlerType { get; }

        MethodInfo Method { get; }

        MockMethodAttribute MockMethod { get; }
    }
}
