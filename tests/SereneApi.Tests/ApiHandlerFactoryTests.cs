using SereneApi.Factories;
using SereneApi.Tests.Handlers;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerFactoryTests
    {

        [Fact]
        public void BuildHandler()
        {
            ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterHandler<UserApiHandler>(o =>
            {
                o.UseSource("http://localhost", "Users");
            });

            UserApiHandler userApi = handlerFactory.Build<UserApiHandler>();

            handlerFactory.Dispose();
        }
    }
}
