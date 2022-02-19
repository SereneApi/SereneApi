using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestPerformer<TContent>
    {
        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse<TContent>> ExecuteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs the request Asynchronously.
        /// </summary>
        Task<IApiResponse<TContent>> ExecuteAsync(Action<IApiRequestOptionsBuilder> optionsBuilder, CancellationToken cancellationToken = default);
    }
}