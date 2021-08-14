using SereneApi.Core.Options;
using SereneApi.Handlers.Rest.Tests.Interfaces;

namespace SereneApi.Handlers.Rest.Tests.Mock
{
    public class CrudApiHandlerWrapper : CrudApiHandler<MockPersonDto, long>, ICrudApi
    {
        public CrudApiHandlerWrapper(IApiOptions options) : base(options)
        {
        }
    }
}
