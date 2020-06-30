using SereneApi.Tests.Interfaces;

namespace SereneApi.Tests.Mock
{
    public class CrudApiHandlerWrapper: CrudApiHandler<MockPersonDto, long>, ICrudApi
    {
        public CrudApiHandlerWrapper(IApiHandlerOptions options) : base(options)
        {
        }
    }
}
