using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Core.Http.Client.Handler
{
    public interface IHandlerFactory
    {
        IReadOnlyList<DelegatingHandler> AdditionalHandlers { get; }
        HttpMessageHandler PrimaryHandler { get; set; }

        void AddHandler(DelegatingHandler handler);

        void AddHandler<THandler>() where THandler : DelegatingHandler;
    }
}