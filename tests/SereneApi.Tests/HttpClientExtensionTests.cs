using SereneApi.Tests.Handlers;
using Shouldly;
using System;
using System.Net.Http;
using Xunit;

namespace SereneApi.Tests
{
    public class HttpClientExtensionTests
    {
        [Fact]
        public void CreateApiHandler()
        {
            string resource = "Users";
            string source = $"http://localhost/api/{resource}";

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(source)
            };

            using (UserApiHandler userApi = client.CreateApiHandler<UserApiHandler>())
            {
                userApi.Source.ShouldBe(new Uri(source));
                userApi.Resource.ShouldBe(resource);
            }
        }
    }
}
