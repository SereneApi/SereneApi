using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Configuration.Provider;

namespace SereneApi.Core.Tests.Configuration.Mocking
{
    public class TestHandlerConfigurationProvider : HandlerConfigurationProvider
    {
        protected override void Configure(IDependencyCollection dependencies)
        {
        }
    }
}