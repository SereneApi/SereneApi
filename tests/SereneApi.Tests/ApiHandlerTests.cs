using Moq;
using SereneApi.Factories;
using SereneApi.Interfaces;
using System.Net.Http;

namespace SereneApi.Tests
{
    public class ApiHandlerTests
    {
        private readonly Mock<HttpClientHandler> _mockClientHandler;

        public void GetRequest()
        {
            IApiHandlerFactory factory = new ApiHandlerFactory();
        }
    }
}
