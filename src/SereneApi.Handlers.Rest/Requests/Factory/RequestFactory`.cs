﻿using SereneApi.Core.Responses;
using SereneApi.Core.Routing;
using SereneApi.Handlers.Rest.Requests.Types;
using SereneApi.Handlers.Rest.Routing;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factory
{
    internal class RequestFactory<TContent> : IApiRequestPerformer<TContent>
    {
        private readonly RestApiHandler _apiHandler;

        private readonly RestApiRequest _restApiRequest;

        public RequestFactory(RestApiHandler apiHandler, RestApiRequest request)
        {
            _apiHandler = apiHandler;
            _restApiRequest = request;
        }

        public Task<IApiResponse<TContent>> ExecuteAsync()
        {
            GenerateRoute();

            return _apiHandler.PerformRequestAsync<TContent>(_restApiRequest);
        }

        private void GenerateRoute()
        {
            RouteFactory routeFactory = (RouteFactory)_apiHandler.Options.Dependencies.GetDependency<IRouteFactory>();

            _restApiRequest.Endpoint = routeFactory.BuildEndPoint(_restApiRequest);
            _restApiRequest.Route = routeFactory.BuildRoute(_restApiRequest);
        }
    }
}
