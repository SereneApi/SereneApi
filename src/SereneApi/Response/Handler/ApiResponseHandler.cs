using SereneApi.Request;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Response.Handler
{
    internal sealed class ApiResponseHandler : IApiResponseHandler
    {
        public Task<IApiResponse> HandleSuccessfulResponseAsync(IApiRequest apiRequest, HttpResponseMessage httpResponse, TimeSpan responseTime, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IApiResponse> HandleFailedResponseAsync(IApiRequest apiRequest, HttpResponseMessage httpResponse, TimeSpan responseTime, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
