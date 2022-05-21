using SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors;
using SereneApi.Extensions.Mocking.Rest.Handler;
using System;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Exceptions
{
    public class InvalidMockRestApiHandlerReturnTypeException : Exception
    {
        internal InvalidMockRestApiHandlerReturnTypeException(IMockHandlerDescriptor descriptor, Type returnType) : base($"{descriptor.HandlerType.Name}.{descriptor.Method.Name} returned an invalid Type {returnType.Name}. Only MockRestApiHandler may only return the Type {nameof(IMockResult)} or {typeof(Task<IMockResult>)}")
        {
        }
    }
}
