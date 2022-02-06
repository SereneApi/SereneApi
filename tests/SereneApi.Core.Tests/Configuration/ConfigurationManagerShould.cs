using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Settings;
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
            ApiConfigurationManager configuration = new ApiConfigurationManager();

            configuration.AddApiConfiguration<TestApiHandler>(c =>
            {
                c.SetSource("http://localhost", "Client", "api/");
            });

            IApiSettings<TestApiHandler> settings = Should.NotThrow(configuration.BuildApiOptions<TestApiHandler>);

            settings.ShouldNotBeNull();
        }
    }
}