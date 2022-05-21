using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Tests.Configuration.Mocking;
using Shouldly;
using System;
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

            configuration.AddApiConfiguration<OtherTestApiHandler>(c =>
            {
                c.SetSource("http://localhost", "Other", "api/");
            });

            IApiSettings<TestApiHandler> testSetting = Should.NotThrow(configuration.BuildApiSettings<TestApiHandler>);
            IApiSettings<OtherTestApiHandler> otherTestSetting = Should.NotThrow(configuration.BuildApiSettings<OtherTestApiHandler>);

            testSetting.ShouldNotBeNull();
            testSetting.Connection.BaseAddress.ShouldBe(new Uri("http://localhost"));
            testSetting.Connection.Resource.ShouldBe("Client");
            testSetting.Connection.ResourcePath.ShouldBe("api");

            otherTestSetting.ShouldNotBeNull();
            otherTestSetting.Connection.BaseAddress.ShouldBe(new Uri("http://localhost"));
            otherTestSetting.Connection.Resource.ShouldBe("Other");
            otherTestSetting.Connection.ResourcePath.ShouldBe("api");
        }
    }
}