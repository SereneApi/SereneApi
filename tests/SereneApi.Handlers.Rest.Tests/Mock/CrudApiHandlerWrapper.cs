using SereneApi.Core.Configuration.Settings;
using SereneApi.Handlers.Rest.Tests.Interfaces;

namespace SereneApi.Handlers.Rest.Tests.Mock
{
    public class CrudApiHandlerWrapper : CrudApiHandler<MockPersonDto, long>, ICrudApi
    {
        public CrudApiHandlerWrapper(IApiSettings<CrudApiHandlerWrapper> settings) : base(settings)
        {
        }
    }
}