using SereneApi.Abstractions.Options;

namespace SereneApi.Extensions.DependencyInjection.Tests
{
    public class TestApiHandler: BaseApiHandler, ITestApi
    {
        public new IApiOptions Options => base.Options;

        public TestApiHandler(IApiOptions<ITestApi> options) : base(options)
        {
        }
    }
}
