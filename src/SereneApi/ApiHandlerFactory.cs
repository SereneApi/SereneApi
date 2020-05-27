using DeltaWare.SereneApi.Interfaces;
using System;
using System.Collections.Generic;

namespace DeltaWare.SereneApi
{
    public class ApiHandlerFactory : IApiHandlerFactory
    {
        private readonly Dictionary<Type, IApiHandlerOptions> _handlers = new Dictionary<Type, IApiHandlerOptions>();

        public TApiHandler CreateInstance<TApiHandler>() where TApiHandler : ApiHandler
        {



            throw new NotImplementedException();
        }

        public void RegisterHandler<TApiHandler>(Action<ApiHandlerOptionsBuilder> optionsAction) where TApiHandler : ApiHandler
        {
            Type handlerType = typeof(TApiHandler);

            if (_handlers.ContainsKey(handlerType))
            {
                throw new ArgumentException($"Cannot Register Multiple Instances of {nameof(TApiHandler)}");
            }

            ApiHandlerOptionsBuilder optionsBuilder = new ApiHandlerOptionsBuilder();

            optionsAction.Invoke(optionsBuilder);

            _handlers.Add(handlerType, optionsBuilder.BuildOptions(false));
        }
    }
}
