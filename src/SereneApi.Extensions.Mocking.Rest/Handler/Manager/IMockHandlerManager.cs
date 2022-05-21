using SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors;
using SereneApi.Extensions.Mocking.Rest.Responses;
using System.Net.Http;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Manager
{
    internal interface IMockHandlerManager
    {
        Task<IMockResponse> InvokeHandlerAsync(IMockHandlerDescriptor mockHandler, HttpRequestMessage request, string requestEndpoint);
    }
}
