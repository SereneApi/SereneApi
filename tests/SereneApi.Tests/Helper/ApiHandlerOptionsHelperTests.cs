using SereneApi.Helpers;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Tests.Helper
{
    public class ApiHandlerOptionsHelperTests
    {
        [Theory]
        // Normal tests checking for / protection
        [InlineData("http://localhost:8080", "TestSource1/", "http://localhost:8080/api/TestSource1/")]
        [InlineData("http://localhost:80", "TestSource1", "http://localhost:80/api/TestSource1/")]
        [InlineData("http://localhost", "TestSource2/Extra1/", "http://localhost/api/TestSource2/Extra1/")]
        [InlineData("http://localhost", "TestSource2/Extra1", "http://localhost/api/TestSource2/Extra1/")]
        public void FormatSourceResource(string source, string resource, string finalSource)
        {
            Uri finalSourceUri = new Uri(finalSource);

            Uri formattedSource = ApiHandlerOptionsHelper.FormatSource(source, resource);

            formattedSource.ShouldBe(finalSourceUri);
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
        public void FormatSourceResourceAndPath(string source, string resource, string resourcePath, string finalSource)
        {
            Uri finalSourceUri = new Uri(finalSource);

            Uri formattedSource = ApiHandlerOptionsHelper.FormatSource(source, resource, resourcePath);

            formattedSource.ShouldBe(finalSourceUri);
        }

        [Fact]
        public void FormatSourceThrowsArgumentException()
        {
            Should.Throw<ArgumentException>(() =>
            {
                ApiHandlerOptionsHelper.FormatSource("", "", "");
            });
        }
    }
}
