using SereneApi.Core.Options;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Tests.Interfaces;

namespace SereneApi.Handlers.Rest.Tests.Mock
{
    public class BaseApiHandlerWrapper : RestApiHandler, IApiHandlerWrapper
    {
        public new IApiRequestMethod MakeRequest => base.MakeRequest;

        public BaseApiHandlerWrapper(IApiOptions options) : base(options)
        {
        }
    }
}