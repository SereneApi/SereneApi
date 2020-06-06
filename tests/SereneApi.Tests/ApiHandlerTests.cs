using SereneApi.Factories;
using SereneApi.Testing;
using SereneApi.Tests.Mock;
using Shouldly;
using System;
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
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponse(r =>
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
            .WithMockResponse(MockPersonDto.John, requestUri);

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
        public async Task SuccessfulGetRequestWithTemplateAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/100/Details");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponse(r =>
            {
                r.StatusCode = HttpStatusCode.OK;
            }, requestUri);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulGetRequestWithTemplateGenericAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/100/Details");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponse(MockPersonDto.John, requestUri);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.John.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.John.Name);
            personResult.Age.ShouldBe(MockPersonDto.John.Age);
        }

        [Fact]
        public async Task SuccessfulGetRequestWithQueryGenericAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons?Age=18&Name=John Smith");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponse(MockPersonDto.John, requestUri);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestWithQueryAsync<MockPersonDto, MockPersonDto>(Method.Get, MockPersonDto.John, dto => new { dto.Age, dto.Name });

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
            .WithMockResponse(r =>
            {
                r.StatusCode = HttpStatusCode.NotFound;
                r.ReasonPhrase = reasonPhrase;
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
            .WithMockResponse(r =>
            {
                r.StatusCode = HttpStatusCode.NotFound;
                r.ReasonPhrase = reasonPhrase;
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

        [Fact]
        public async Task SuccessfulGetRequestWithTimeoutAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
            })
            .WithMockResponse(r =>
            {
                r.StatusCode = HttpStatusCode.OK;
            }, requestUri)
            .WithTimeout(2);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulGetRequestWithTimeoutGenericAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
            })
            .WithMockResponse(MockPersonDto.John, requestUri)
            .WithTimeout(2);

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
        public async Task UnSuccessfulGetRequestWithTimeoutAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
            })
            .WithMockResponse(r =>
            {
                r.StatusCode = HttpStatusCode.NotFound;
                r.ReasonPhrase = reasonPhrase;
            }, requestUri)
            .WithTimeout(2);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task UnSuccessfulGetRequestWithTimeoutGenericAsync()
        {
            Uri requestUri = new Uri("http://localhost/api/Persons/");

            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
            })
            .WithMockResponse(r =>
            {
                r.StatusCode = HttpStatusCode.NotFound;
                r.ReasonPhrase = reasonPhrase;
            }, requestUri)
            .WithTimeout(2);

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();
            response.Result.ShouldBeNull();
        }
    }

}
