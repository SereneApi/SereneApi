using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Configuration;
using SereneApi.Core.Configuration.Provider;
using Shouldly;
using System;
using SereneApi.Handlers.Rest.Configuration;
using Xunit;

namespace SereneApi.Extensions.DependencyInjection.Tests
{
    public class ServicesExtensionShould
    {
        [Fact]
        public void RegisterApi()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.RegisterApi<ITestApi, TestApiHandler>(b =>
            {
                b.SetSource("http://localhost/", "Test");
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api");
            testApi.Connection.Timeout.ShouldBe(30);
            testApi.Connection.RetryAttempts.ShouldBe(0);

            testApi.Settings.Dependencies.TryGetDependency(out ILogger logger).ShouldBeFalse();
        }

        [Fact]
        public void RegisterApiWithDefaultOverride()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AmendConfigurationProvider<RestHandlerConfigurationProvider>(r =>
            {
                r.SetHandlerConfiguration(c =>
                {
                    c.SetResourcePath("api/v2/");
                });

                r.Dependencies.AddScoped<ILogger>(() => new Logger<ITestApi>(new LoggerFactory()));
            });

            serviceCollection.RegisterApi<ITestApi, TestApiHandler>(b =>
            {
                b.SetSource("http://localhost/", "Test");
                b.SetTimeout(60, 2);
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api/v2");
            testApi.Connection.Timeout.ShouldBe(60);
            testApi.Connection.RetryAttempts.ShouldBe(2);

            testApi.Settings.Dependencies.TryGetDependency(out ILogger logger).ShouldBeTrue();

            logger.ShouldBeOfType<Logger<ITestApi>>();
        }

        [Fact]
        public void RegisterApiWithDefaultOverrideAndExtension()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AmendConfigurationProvider<RestHandlerConfigurationProvider>(r =>
            {
                r.SetHandlerConfiguration(c =>
                {
                    c.SetResourcePath("api/v2/");
                });
            });

            serviceCollection.RegisterApi<ITestApi, TestApiHandler>(b =>
            {
                b.SetSource("http://localhost/", "Test");
            });

            serviceCollection.ExtendApi<TestApiHandler>(c =>
            {
                c.Dependencies.AddScoped<ILogger>(() => new Logger<ITestApi>(new LoggerFactory()));
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api/v2");
            testApi.Connection.Timeout.ShouldBe(30);
            testApi.Connection.RetryAttempts.ShouldBe(0);

            testApi.Settings.Dependencies.TryGetDependency(out ILogger logger).ShouldBeTrue();

            logger.ShouldBeOfType<Logger<ITestApi>>();
        }

        [Fact]
        public void RegisterApiWithDefaultOverrideAndExtensionBuilder()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AmendConfigurationProvider<RestHandlerConfigurationProvider>(r =>
            {
                r.SetHandlerConfiguration(c =>
                {
                    c.SetTimeout(60);
                    c.SetRetryAttempts(2);
                });
            });

            serviceCollection.RegisterApi<ITestApi, TestApiHandler>(b =>
            {
                b.SetSource("http://localhost/", "Test");
            });

            serviceCollection.ExtendApi<TestApiHandler>(r =>
            {
                // For testing purposes only, but this is how implementation should be done for extensions.
                r.Dependencies.AddScoped<ILogger>(() => new Logger<ITestApi>(new LoggerFactory()));
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api");
            testApi.Connection.Timeout.ShouldBe(60);
            testApi.Connection.RetryAttempts.ShouldBe(2);

            testApi.Settings.Dependencies.TryGetDependency(out ILogger logger).ShouldBeTrue();

            logger.ShouldBeOfType<Logger<ITestApi>>();
        }
    }
}