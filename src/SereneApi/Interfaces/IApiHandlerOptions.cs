using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace DeltaWare.SereneApi.Interfaces
{
    public interface IApiHandlerOptions
    {
        ILogger Logger { get; }

        IQueryFactory QueryFactory { get; }

        /// <summary>
        /// The <see cref="HttpClient"/> that the <see cref="ApiHandler"/> will use to make requests.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// The numbers of times the <see cref="ApiHandler"/> will re-attempt the connection.
        /// </summary>
        uint RetryCount { get; }
    }
}
