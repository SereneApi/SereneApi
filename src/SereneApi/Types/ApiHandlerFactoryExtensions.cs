using SereneApi.Interfaces;
using System;

namespace SereneApi.Types
{
    public class ApiHandlerFactoryExtensions<THandler> : CoreOptions, IApiHandlerFactoryExtensions
    {
        public Type HandlerType => typeof(THandler);
    }
}
