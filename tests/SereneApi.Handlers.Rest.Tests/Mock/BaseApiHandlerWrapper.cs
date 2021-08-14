using SereneApi.Core.Options;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Tests.Interfaces;

namespace SereneApi.Handlers.Rest.Tests.Mock
{
    public class BaseApiHandlerWrapper : RestApiHandler, IApiHandlerWrapper
    {
        public BaseApiHandlerWrapper(IApiOptions options) : base(options)
        {
        }

        public new IApiRequestFactory MakeRequest => base.MakeRequest;
    }
}
