using SereneApi.Factories;
using SereneApi.Interfaces;
using SereneApi.Tests.Mock.Handlers;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Tests.Factories
{
    public class ApiHandlerFactoryTests
    {
        [Theory]
        [InlineData("http://localhost:8080", "TestSource1/", "http://localhost:8080/api/TestSource1/")]
        [InlineData("http://localhost:80", "TestSource1", "http://localhost:80/api/TestSource1/")]
        [InlineData("http://localhost", "TestSource2/Extra1/", "http://localhost/api/TestSource2/Extra1/")]
        [InlineData("http://localhost", "TestSource2/Extra1", "http://localhost/api/TestSource2/Extra1/")]
        public void RegisterHandlerSourceResource(string source, string resource, string finalSource)
        {
            Uri sourceUri = new Uri(finalSource);

            ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterHandlerOptions<UserApiHandler>(o =>
            {
                o.UseSource(source, resource);
            });

            UserApiHandler userApi = handlerFactory.Build<UserApiHandler>();

            userApi.Source.ShouldBe(sourceUri);
            userApi.Resource.ShouldBe(resource);

            handlerFactory.Dispose();
        }

        [Theory]
        // Normal tests checking for / protection
        [InlineData("http://localhost:8080", "TestSource1/", "apiv2/", "http://localhost:8080/apiv2/TestSource1/")]
        [InlineData("http://localhost:8080", "TestSource1", "apiv2", "http://localhost:8080/apiv2/TestSource1/")]
        [InlineData("http://localhost:80", "TestSource2/", "api/v2/", "http://localhost:80/api/v2/TestSource2/")]
        [InlineData("http://localhost:80", "TestSource2", "api/v2", "http://localhost:80/api/v2/TestSource2/")]
        // Empty string is provided so default is not used and thus removed.
        [InlineData("http://localhost", "TestSource3/", "", "http://localhost/TestSource3/")]
        [InlineData("http://localhost", "TestSource3", "", "http://localhost/TestSource3/")]
        // Space or null is provided, uses default value.
        [InlineData("http://localhost", "TestSource4/", " ", "http://localhost/api/TestSource4/")]
        [InlineData("http://localhost", "TestSource4", " ", "http://localhost/api/TestSource4/")]
        public void RegisterHandlerSourceResourceAndPath(string source, string resource, string resourcePath, string finalSource)
        {
            Uri sourceUri = new Uri(finalSource);

            ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterHandlerOptions<UserApiHandler>(o =>
            {
                o.UseSource(source, resource, resourcePath);
            });

            UserApiHandler userApi = handlerFactory.Build<UserApiHandler>();

            userApi.Source.ShouldBe(sourceUri);
            userApi.Resource.ShouldBe(resource);

            handlerFactory.Dispose();
        }

        [Fact]
        public void RegisterDuplicateHandlers()
        {
            ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterHandlerOptions<UserApiHandler>(o =>
            {
                o.UseSource("http://localhost", "Users");
            });

            Should.Throw<ArgumentException>(() =>
            {
                handlerFactory.RegisterHandlerOptions<UserApiHandler>(o =>
                {
                    o.UseSource("http://localhost", "Users");
                });
            });
        }

        [Fact]
        public void BuildHandlerNoRegistration()
        {
            IApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            Should.Throw<ArgumentException>(() =>
            {
                handlerFactory.Build<UserApiHandler>();
            });
        }
    }
}
