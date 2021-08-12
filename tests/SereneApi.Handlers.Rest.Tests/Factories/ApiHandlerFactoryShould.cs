using SereneApi.Tests.Interfaces;
using SereneApi.Tests.Mock;
using Shouldly;
using System;
using SereneApi.Core.Handler.Factories;
using Xunit;

namespace SereneApi.Tests.Factories
{
    public class ApiHandlerFactoryShould
    {
        [Theory]
        [InlineData("http://localhost:8080", "TestSource1/", "http://localhost:8080/api/TestSource1", "TestSource1")]
        [InlineData("http://localhost:443", "TestSource1", "http://localhost:443/api/TestSource1", "TestSource1")]
        [InlineData("http://localhost", "TestSource2/Extra1/", "http://localhost/api/TestSource2/Extra1", "TestSource2/Extra1")]
        [InlineData("http://localhost", "TestSource2/Extra1", "http://localhost/api/TestSource2/Extra1", "TestSource2/Extra1")]
        public void RegisterHandlerSourceResource(string source, string resource, string expectedSource, string expectedResource)
        {
            ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
            });

            using IApiHandlerWrapper apiHandlerWrapperWrapper = apiFactory.Build<IApiHandlerWrapper>();

            apiHandlerWrapperWrapper.Connection.Source.ShouldBe(expectedSource);
            apiHandlerWrapperWrapper.Connection.Resource.ShouldBe(expectedResource);

            apiFactory.Dispose();
        }

        [Theory]
        // Normal tests checking for / protection
        [InlineData("http://localhost:8080", "TestSource1/", "apiv2/", "http://localhost:8080/apiv2/TestSource1", "TestSource1")]
        [InlineData("http://localhost:8080", "TestSource1", "apiv2", "http://localhost:8080/apiv2/TestSource1", "TestSource1")]
        [InlineData("http://localhost:443", "TestSource2/", "api/v2/", "http://localhost:443/api/v2/TestSource2", "TestSource2")]
        [InlineData("http://localhost:443", "TestSource2", "api/v2", "http://localhost:443/api/v2/TestSource2", "TestSource2")]
        // Empty string is provided so default is not used and thus removed.
        [InlineData("http://localhost", "TestSource3/", "", "http://localhost/TestSource3", "TestSource3")]
        [InlineData("http://localhost", "TestSource3", "", "http://localhost/TestSource3", "TestSource3")]
        // Space or null is provided, uses default value.
        [InlineData("http://localhost", "TestSource4/", " ", "http://localhost/api/TestSource4", "TestSource4")]
        [InlineData("http://localhost/Path", "TestSource4", " ", "http://localhost/Path/api/TestSource4", "TestSource4")]
        public void RegisterHandlerSourceResourceAndPath(string source, string resource, string resourcePath, string expectedSource, string expectedResource)
        {
            ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource, resourcePath);
            });

            using IApiHandlerWrapper apiHandlerWrapperWrapper = apiFactory.Build<IApiHandlerWrapper>();

            apiHandlerWrapperWrapper.Connection.Source.ShouldBe(expectedSource);
            apiHandlerWrapperWrapper.Connection.Resource.ShouldBe(expectedResource);

            apiFactory.Dispose();
        }

        [Fact]
        public void RegisterDuplicateHandlers()
        {
            ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource("http://localhost", "Users");
            });

            Should.Throw<ArgumentException>(() =>
            {
                apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
                {
                    o.SetSource("http://localhost", "Users");
                });
            });
        }

        [Fact]
        public void BuildHandlerNoRegistration()
        {
            using ApiFactory apiFactory = new ApiFactory();

            Should.Throw<ArgumentException>(() =>
            {
                apiFactory.Build<IApiHandlerWrapper>();
            });
        }
    }
}
