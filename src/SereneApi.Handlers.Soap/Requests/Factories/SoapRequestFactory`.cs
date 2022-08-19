using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
using SereneApi.Handlers.Soap.Requests.Types;
using SereneApi.Handlers.Soap.Routing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    internal class SoapRequestFactory<TResponse> : IRequestPerformer<TResponse> where TResponse : class
    {
        private readonly SoapApiHandler _apiHandler;

        private readonly SoapApiRequest _apiRequest;

        private readonly IRouteFactory _routeFactory;

        public SoapRequestFactory(SoapApiHandler apiHandler, SoapApiRequest apiRequest, IRouteFactory routeFactory)
        {
            _apiHandler = apiHandler;
            _apiRequest = apiRequest;
            _routeFactory = routeFactory;
        }

        public Task<IApiResponse<TResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _apiRequest.Route = _routeFactory.BuildRoute(_apiRequest);
            _apiRequest.Url = _routeFactory.GetUrl(_apiRequest);

            return _apiHandler.PerformRequestAsync<TResponse>(_apiRequest, ApiRequestOptions.Default, cancellationToken);
        }

        public Task<IApiResponse<TResponse>> ExecuteAsync(Action<IApiRequestOptionsBuilder> optionsBuilder, CancellationToken cancellationToken = default)
        {
            _apiRequest.Route = _routeFactory.BuildRoute(_apiRequest);
            _apiRequest.Url = _routeFactory.GetUrl(_apiRequest);

            ApiRequestOptions options = ApiRequestOptions.Default;

            optionsBuilder.Invoke(options);

            return _apiHandler.PerformRequestAsync<TResponse>(_apiRequest, options, cancellationToken);
        }
    }
}