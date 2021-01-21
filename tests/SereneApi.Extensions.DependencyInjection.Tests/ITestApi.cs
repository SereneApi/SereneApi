using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Options;

namespace SereneApi.Extensions.DependencyInjection.Tests
{
    public interface ITestApi: IApiHandler
    {
        IApiOptions Options { get; }
    }
}
