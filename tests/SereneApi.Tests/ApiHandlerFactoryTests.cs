using DeltaWare.SereneApi;
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

            handlerFactory.RegisterHandler<UsersApiHandler>(o =>
            {
                o.UseSource("http://localhost", "Users");
            });

            UsersApiHandler usersApi = handlerFactory.Build<UsersApiHandler>();

            handlerFactory.Dispose();
        }
    }
}
