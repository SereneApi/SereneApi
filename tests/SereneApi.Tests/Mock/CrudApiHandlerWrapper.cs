using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Handler.Options;
using SereneApi.Tests.Interfaces;

namespace SereneApi.Tests.Mock
{
    public class CrudApiHandlerWrapper: CrudApiHandler<MockPersonDto, long>, ICrudApi
    {
        public CrudApiHandlerWrapper(IOptions options) : base(options)
        {
        }
    }
}
