using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestPerformer
    {
        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse> ExecuteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse> ExecuteAsync(Action<IApiRequestOptionsBuilder> optionsBuilder, CancellationToken cancellationToken = default);
    }
}