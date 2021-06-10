using SereneApi.Abstractions.Options;
using SereneApi.Requests;
using SereneApi.Tests.Interfaces;

namespace SereneApi.Tests.Mock
{
    public class BaseApiHandlerWrapper : BaseApiHandler, IApiHandlerWrapper
    {
        public BaseApiHandlerWrapper(IApiOptions options) : base(options)
        {
        }

        public new IApiRequestBuilder MakeRequest => base.MakeRequest;
    }
}
