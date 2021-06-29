using SereneApi.Abstractions.Requests.Builder;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Routing;
using SereneApi.Requests.Types;
using System.Threading.Tasks;

namespace SereneApi.Requests.Builder
{
    internal class RequestBuilder<TContent> : IApiRequestPerformer<TContent>
    {
        private readonly BaseApiHandler _apiHandler;

        private readonly ApiRequest _apiRequest;

        public RequestBuilder(BaseApiHandler apiHandler, ApiRequest request)
        {
            _apiHandler = apiHandler;
            _apiRequest = request;
        }

        public Task<IApiResponse<TContent>> ExecuteAsync()
        {
            GenerateRoute();

            return _apiHandler.PerformRequestAsync<TContent>(_apiRequest);
        }

        private void GenerateRoute()
        {
            IRouteFactory routeFactory = _apiHandler.Options.Dependencies.GetDependency<IRouteFactory>();

            _apiRequest.Endpoint = routeFactory.BuildEndPoint(_apiRequest);
            _apiRequest.Route = routeFactory.BuildRoute(_apiRequest);
        }
    }
}
