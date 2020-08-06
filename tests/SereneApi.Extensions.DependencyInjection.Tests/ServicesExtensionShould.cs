using DeltaWare.Dependencies.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Options;
using Shouldly;
using System;
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
                b.UseSource("http://localhost/", "Test");
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api/");
            testApi.Connection.Timeout.ShouldBe(30);
            testApi.Connection.RetryAttempts.ShouldBe(0);

            testApi.Options.RetrieveDependency(out ILogger logger).ShouldBeFalse();
        }

        [Fact]
        public void RegisterApiWithDefaultOverride()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.ConfigureSereneApi(r =>
            {
                r.ResourcePath = "api/v2/";
                r.AddDependencies(d => d.AddScoped<ILogger>(() => new Logger<ITestApi>(new LoggerFactory())));
            });

            serviceCollection.RegisterApi<ITestApi, TestApiHandler>(b =>
            {
                b.UseSource("http://localhost/", "Test");
                b.SetTimeout(60, 2);
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api/v2/");
            testApi.Connection.Timeout.ShouldBe(60);
            testApi.Connection.RetryAttempts.ShouldBe(2);

            testApi.Options.RetrieveDependency(out ILogger logger).ShouldBeTrue();

            logger.ShouldBeOfType<Logger<ITestApi>>();
        }


        [Fact]
        public void RegisterApiWithDefaultOverrideAndExtension()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.ConfigureSereneApi(r =>
            {
                r.ResourcePath = "api/v2/";
            });

            serviceCollection.RegisterApi<ITestApi, TestApiHandler>(b =>
            {
                b.UseSource("http://localhost/", "Test");
            });

            ((ICoreOptions)serviceCollection.ExtendApi<ITestApi>()).Dependencies
                .AddScoped<ILogger>(() => new Logger<ITestApi>(new LoggerFactory()));

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api/v2/");
            testApi.Connection.Timeout.ShouldBe(30);
            testApi.Connection.RetryAttempts.ShouldBe(0);

            testApi.Options.RetrieveDependency(out ILogger logger).ShouldBeTrue();

            logger.ShouldBeOfType<Logger<ITestApi>>();
        }

        [Fact]
        public void RegisterApiWithDefaultOverrideAndExtensionBuilder()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.ConfigureSereneApi(r =>
            {
                r.Timeout = 60;
                r.RetryCount = 2;
            });

            serviceCollection.RegisterApi<ITestApi, TestApiHandler>(b =>
            {
                b.UseSource("http://localhost/", "Test");
            });

            serviceCollection.ExtendApi<ITestApi>(r =>
            {
                // For testing purposes only, but this is how implementation should be done for extensions.
                ((ICoreOptions)r).Dependencies.AddScoped<ILogger>(() => new Logger<ITestApi>(new LoggerFactory()));
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            ITestApi testApi = Should.NotThrow(() => serviceProvider.GetRequiredService<ITestApi>());

            testApi.ShouldNotBeNull();
            testApi.Connection.BaseAddress.ShouldBe(new Uri("http://localhost/"));
            testApi.Connection.Resource.ShouldBe("Test");
            testApi.Connection.ResourcePath.ShouldBe("api/");
            testApi.Connection.Timeout.ShouldBe(60);
            testApi.Connection.RetryAttempts.ShouldBe(2);

            testApi.Options.RetrieveDependency(out ILogger logger).ShouldBeTrue();

            logger.ShouldBeOfType<Logger<ITestApi>>();
        }
    }
}
