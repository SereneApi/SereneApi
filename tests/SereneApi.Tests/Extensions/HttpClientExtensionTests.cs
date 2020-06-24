using SereneApi.Tests.Mock;
using Shouldly;
using System;
using System.Net.Http;
using Xunit;

namespace SereneApi.Tests.Extensions
{
    public class HttpClientExtensionTests
    {
        [Theory]
        [InlineData("http://localhost/api/Users/")]
        [InlineData("http://localhost:8080/api/Vehicles/")]
        [InlineData("http://localhost/Values/")]
        public void CreateApiHandler(string sourceString)
        {
            Uri source = new Uri(sourceString);

            using HttpClient client = new HttpClient
            {
                BaseAddress = source
            };

            using ApiHandlerWrapper api = client.CreateApiHandler<ApiHandlerWrapper>();

            api.Connection.BaseAddress.ShouldBe(source);
        }

        [Theory]
        [InlineData("http://localhost/api/Users")]
        [InlineData("http://localhost:8080/api/Vehicles")]
        [InlineData("http://localhost/Values")]
        public void CreateApiHandlerThrowsArgumentException(string sourceString)
        {
            Uri source = new Uri(sourceString);

            using HttpClient client = new HttpClient
            {
                BaseAddress = source
            };

            Should.Throw<ArgumentException>(() =>
            {
                client.CreateApiHandler<ApiHandlerWrapper>();
            });
        }
    }
}
