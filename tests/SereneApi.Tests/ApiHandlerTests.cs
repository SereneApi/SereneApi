using SereneApi.Factories;
using SereneApi.Testing;
using SereneApi.Tests.Mock;
using Shouldly;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerTests
    {
        [Fact]
        public async Task SuccessfulGetRequestAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                })
                .WithMoqResponse(response =>
                {
                    response.StatusCode = HttpStatusCode.OK;
                });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulGetRequestGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                })
                .WithMoqResponse(MockPersonDto.John);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.John.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.John.Name);
            personResult.Age.ShouldBe(MockPersonDto.John.Age);
        }


        [Fact]
        public async Task UnSuccessfulGetRequestAsync()
        {
            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                })
                .WithMoqResponse(response =>
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = reasonPhrase;
                });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task UnSuccessfulGetRequestGenericAsync()
        {
            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                })
                .WithMoqResponse(response =>
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = reasonPhrase;
                });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();
            response.Result.ShouldBeNull();
        }
    }
}
