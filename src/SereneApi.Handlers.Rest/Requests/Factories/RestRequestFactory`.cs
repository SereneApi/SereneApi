using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Requests.Options;
using SereneApi.Core.Http.Responses;
using SereneApi.Handlers.Rest.Routing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public class RestRequestFactory<TContent> : IApiRequestPerformer<TContent>
    {
        private readonly IApiHandler _apiHandler;

        private readonly RestApiRequest _restApiRequest;

        public RestRequestFactory(IApiHandler apiHandler, RestApiRequest request)
        {
            _apiHandler = apiHandler;
            _restApiRequest = request;
        }

        public Task<IApiResponse<TContent>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            GenerateRoute();

            return _apiHandler.PerformRequestAsync<TContent>(_restApiRequest, ApiRequestOptions.Default, cancellationToken);
        }

        public Task<IApiResponse<TContent>> ExecuteAsync(Action<IApiRequestOptionsBuilder> optionsBuilder, CancellationToken cancellationToken = default)
        {
            GenerateRoute();

            ApiRequestOptions options = ApiRequestOptions.Default;

            optionsBuilder.Invoke(options);

            return _apiHandler.PerformRequestAsync<TContent>(_restApiRequest, options, cancellationToken);
        }

        protected virtual void GenerateRoute()
        {
            RouteFactory routeFactory = (RouteFactory)_apiHandler.Settings.Dependencies.GetRequiredDependency<IRouteFactory>();

            _restApiRequest.Endpoint = routeFactory.BuildEndPoint(_restApiRequest);
            _restApiRequest.Route = routeFactory.BuildRoute(_restApiRequest);
            _restApiRequest.Url = routeFactory.GetUrl(_restApiRequest);
        }
    }
}