using SereneApi.Core.Responses;
using SereneApi.Handlers.Soap.Requests.Types;
using System.Threading.Tasks;

namespace SereneApi.Handlers.Soap.Requests.Factories
{
    public class ApiRequestFactory<TResponse> : IApiRequestPerformer<TResponse> where TResponse : class
    {
        private readonly SoapApiHandler _apiHandler;

        private readonly SoapApiRequest _apiRequest;

        public ApiRequestFactory(SoapApiHandler apiHandler, SoapApiRequest apiRequest)
        {
            _apiHandler = apiHandler;
            _apiRequest = apiRequest;
        }

        public Task<IApiResponse<TResponse>> ExecuteAsync()
        {
            return _apiHandler.PerformRequestAsync<TResponse>(_apiRequest);
        }
    }
}
