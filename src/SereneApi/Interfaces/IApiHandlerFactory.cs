using System;

namespace DeltaWare.SereneApi.Interfaces
{
    internal interface IApiHandlerFactory
    {
        TApiHandler Build<TApiHandler>() where TApiHandler : ApiHandler;

        void RegisterHandler<TApiHandler>(Action<ApiHandlerOptionsBuilder> optionsAction) where TApiHandler : ApiHandler;
    }
}
