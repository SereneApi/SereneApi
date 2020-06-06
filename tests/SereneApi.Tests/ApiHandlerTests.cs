using System;
using SereneApi.Factories;
using SereneApi.Testing;
using SereneApi.Tests.Mock;
using Shouldly;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerTests
    {
        [Fact]
        public async Task SuccessfulGetRequestAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                })
                .WithMoqResponse(r =>
                {
                    r.StatusCode = HttpStatusCode.OK;
                }, requestUri);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulGetRequestGenericAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                })
                .WithMoqResponse(MockPersonDto.John, requestUri);

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
            Uri requestUri = new Uri("http://localhost/api/Persons/");

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
                }, requestUri);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task UnSuccessfulGetRequestGenericAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

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
                }, requestUri);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();
            response.Result.ShouldBeNull();
        }

        [Fact]
        public async Task TimedOutRequest0RetriesAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                })
                .WithTimeoutResponse();

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe("The Request Timed Out; Retry limit reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Result.ShouldBeNull();
        }

        [Fact]
        public async Task TimedOutRequest3RetriesAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
                {
                    builder.UseSource("http://localhost", "Persons");
                    builder.SetRetryOnTimeout(3);
                })
                .WithTimeoutResponse();

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe("The Request Timed Out; Retry limit reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Result.ShouldBeNull();
        }
    }

}
