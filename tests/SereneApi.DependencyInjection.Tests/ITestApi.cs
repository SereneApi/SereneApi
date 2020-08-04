using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;

namespace SereneApi.DependencyInjection.Tests
{
    public interface ITestApi: IApiHandler
    {
        IApiOptions Options { get; }
    }
}
