using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;

namespace SereneApi.Extensions.DependencyInjection.Tests
{
    public interface ITestApi : IApiHandler
    {
        IApiSettings Settings { get; }
    }
}