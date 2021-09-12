using SereneApi.Core.Responses;
using SereneApi.Handlers.Soap.Requests.Types;
using SereneApi.Handlers.Soap.Routing;
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

        public IApiResponse<TResponse> Execute()
        {
            _apiRequest.Route = _routeFactory.BuildRoute(_apiRequest);

            return _apiHandler.PerformRequest<TResponse>(_apiRequest);
        }

        public Task<IApiResponse<TResponse>> ExecuteAsync()
        {
            _apiRequest.Route = _routeFactory.BuildRoute(_apiRequest);

            return _apiHandler.PerformRequestAsync<TResponse>(_apiRequest);
        }
    }
}