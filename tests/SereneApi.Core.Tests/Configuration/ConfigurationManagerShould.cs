using SereneApi.Core.Configuration;
using SereneApi.Core.Options.Factories;
using SereneApi.Core.Tests.Configuration.Mocking;
using Shouldly;
using Xunit;

namespace SereneApi.Core.Tests.Configuration
{
    public class ConfigurationManagerShould
    {
        [Fact]
        public void ContainTestConfigurationFactory()
        {
            ConfigurationManager configuration = new ConfigurationManager();

            ApiOptionsFactory<TestApiHandler> options = Should.NotThrow(() => configuration.BuildApiOptionsFactory<TestApiHandler>());

            options.ShouldNotBeNull();
        }
    }
}