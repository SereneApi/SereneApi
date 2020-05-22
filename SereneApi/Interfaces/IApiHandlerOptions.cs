using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DeltaWare.SereneApi.Interfaces
{
    public interface IApiHandlerOptions
    {
        /// <summary>
        /// The API Source location
        /// EG: "http://SomeHost.com.au"
        /// </summary>
        Uri Source { get; }

        /// <summary>
        /// The API <see cref="Resource"/> to be used  by the <see cref="ApiHandler"/> for API requests
        /// EG: "Members", by default api/ is appended to the front therefore Members would become api/Members.
        /// To change this, supply a <see cref="ResourcePrecursor"/>
        /// </summary>
        string Resource { get; }

        /// <summary>
        /// The API <see cref="ResourcePrecursor"/> will be appended to the front of the <see cref="Resource"/> Value.
        /// By default "api/" is being used
        /// </summary>
        string ResourcePrecursor { get; }

        ILoggerFactory LoggerFactory { get; }

        IQueryFactory QueryFactory { get; }

        IServiceProvider ServiceProvider { get; }

        Type HandlerType { get; }

        /// <summary>
        /// Supplies the the HttpClients to be used for requests.
        /// </summary>
        TimeSpan Timeout { get; }

        HttpClient HttpClient { get; }

        uint RetryCount { get; }

        Action<HttpRequestHeaders> RequestHeaderBuilder { get; }
    }
}
