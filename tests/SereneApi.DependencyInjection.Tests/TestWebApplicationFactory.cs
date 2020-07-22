using DependencyInjection.API;
using DependencyInjection.WebUi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using SereneApi.Extensions.DependencyInjection;
using SereneApi.Extensions.Mocking;

namespace SereneApi.DependencyInjection.Tests
{
    public class TestWebApplicationFactory: WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.ExtendApi<IStudentApi>().WithMockResponse(r =>
                {

                });
            });
        }
    }
}
