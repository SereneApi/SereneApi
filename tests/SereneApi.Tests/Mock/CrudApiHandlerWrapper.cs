using SereneApi.Abstractions.Options;
using SereneApi.Tests.Interfaces;

namespace SereneApi.Tests.Mock
{
    public class CrudApiHandlerWrapper: CrudApiHandler<MockPersonDto, long>, ICrudApi
    {
        public CrudApiHandlerWrapper(IApiOptions apiOptions) : base(apiOptions)
        {
        }
    }
}
