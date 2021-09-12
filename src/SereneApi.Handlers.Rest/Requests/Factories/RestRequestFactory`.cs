using SereneApi.Core.Responses;
using SereneApi.Handlers.Rest.Requests.Types;
using SereneApi.Handlers.Rest.Routing;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public class RestRequestFactory<TContent> : IApiRequestPerformer<TContent>
    {
        private readonly RestApiHandler _apiHandler;

        private readonly RestApiRequest _restApiRequest;

        public RestRequestFactory(RestApiHandler apiHandler, RestApiRequest request)
        {
            _apiHandler = apiHandler;
            _restApiRequest = request;
        }

        public IApiResponse<TContent> Execute()
        {
            GenerateRoute();

            return _apiHandler.PerformRequest<TContent>(_restApiRequest);
        }

        public Task<IApiResponse<TContent>> ExecuteAsync()
        {
            GenerateRoute();

            return _apiHandler.PerformRequestAsync<TContent>(_restApiRequest);
        }

        protected virtual void GenerateRoute()
        {
            RouteFactory routeFactory = (RouteFactory)_apiHandler.Options.Dependencies.GetDependency<IRouteFactory>();

            _restApiRequest.Endpoint = routeFactory.BuildEndPoint(_restApiRequest);
            _restApiRequest.Route = routeFactory.BuildRoute(_restApiRequest);
        }
    }
}