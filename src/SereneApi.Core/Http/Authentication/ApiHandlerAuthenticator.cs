using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Handler.Factories;
using SereneApi.Core.Http.Responses;
using System;
using System.Threading.Tasks;

namespace SereneApi.Core.Http.Authentication
{
    public abstract class ApiHandlerAuthenticator<TApi, TDto> where TApi : class, IDisposable where TDto : class
    {
        private readonly Func<TApi, Task<IApiResponse<TDto>>> _callApi;
        private readonly Func<TDto, TokenAuthResult> _extractToken;
        protected IDependencyProvider Dependencies { get; }

        protected ILogger Logger { get; }

        protected ApiHandlerAuthenticator(IDependencyProvider dependencies, Func<TApi, Task<IApiResponse<TDto>>> callApi, Func<TDto, TokenAuthResult> extractToken)
        {
            _callApi = callApi;
            _extractToken = extractToken;

            Dependencies = dependencies;

            Logger = Dependencies.GetDependency<ILogger>();
        }

        protected virtual TApi GetApi()
        {
            if (Dependencies.TryGetDependency(out IApiFactory handlerFactory))
            {
                return handlerFactory.Build<TApi>();
            }

            if (Dependencies.TryGetDependency(out IServiceProvider services))
            {
                TApi api = (TApi)services.GetService(typeof(TApi));

                if (api != null)
                {
                    return api;
                }
            }

            Logger?.LogError(Logging.EventIds.DependencyNotFound, Logging.Messages.DependencyNotFound, nameof(TApi));

            throw new NullReferenceException($"Could not retrieve an instance of {nameof(TApi)}");
        }

        protected async Task<TokenAuthResult> RequestAccessTokenAsync(bool disposeApi = true)
        {
            TApi api = GetApi();

            IApiResponse<TDto> response = await _callApi.Invoke(api);

            if (disposeApi)
            {
                api.Dispose();
            }

            response.ThrowOnFail();

            TokenAuthResult token = _extractToken.Invoke(response.Data);

            if (token == null)
            {
                throw new NullReferenceException("No token was retrieved.");
            }

            return token;
        }
    }
}