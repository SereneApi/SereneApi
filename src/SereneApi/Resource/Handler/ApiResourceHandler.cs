using SereneApi.Request;
using SereneApi.Request.Handler;
using System.Threading.Tasks;

namespace SereneApi.Resource.Handler
{
    internal sealed class ApiResourceHandler
    {
        private readonly IApiRequestHandler _requestHandler;

        public async Task ExecuteAsync(IApiRequest request)
        {
            await _requestHandler.ExecuteAsync(request);
        }
    }
}
