using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DeltaWare.SereneApi.Interfaces
{
    public interface IApiHandlerOptions
    {
        ILogger Logger { get; }

        IQueryFactory QueryFactory { get; }

        Type HandlerType { get; }

        HttpClient HttpClient { get; }

        uint RetryCount { get; }
    }
}
