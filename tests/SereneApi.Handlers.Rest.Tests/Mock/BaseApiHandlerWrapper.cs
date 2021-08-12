using SereneApi.Core.Options;
using SereneApi.Handlers.Rest;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Tests.Interfaces;

namespace SereneApi.Tests.Mock
{
    public class BaseApiHandlerWrapper : RestApiHandler, IApiHandlerWrapper
    {
        public BaseApiHandlerWrapper(IApiOptions options) : base(options)
        {
        }

        public new IApiRequestFactory MakeRequest => base.MakeRequest;
    }
}
