using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking;
using SereneApi.Factories;
using SereneApi.Tests.Mock;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerTests
    {
        [Fact]
        public void GetRequestWithInvalidTemplateA()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<FormatException>(async () =>
            {
                await apiHandler.InPathRequestAsync(Method.Get, "{0}/Friends/{1}", 1);
            });
        }

        [Fact]
        public void GetRequestWithInvalidTemplateB()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<FormatException>(async () =>
            {
                await apiHandler.InPathRequestAsync(Method.Get, "{0}/Friends", 1, 2);
            });
        }

        [Fact]
        public void ExceptionInBodyGet()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync(Method.Get, MockPersonDto.John);
            });
        }

        [Fact]
        public void ExceptionInBodyGetGeneric()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Get, MockPersonDto.John);
            });
        }

        [Fact]
        public void ExceptionInBodyDelete()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync(Method.Delete, MockPersonDto.John);
            });
        }

        [Fact]
        public void ExceptionInBodyDeleteGeneric()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Delete, MockPersonDto.John);
            });
        }

        [Fact]
        public async Task ExceptionDisposedAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            factory.Dispose();

            Should.Throw<ObjectDisposedException>(async () =>
            {
                await apiHandler.InPathRequestAsync(Method.Get);
            });
        }

        [Fact]
        public async Task ExceptionDisposedGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();
            
            factory.Dispose();

            Should.Throw<ObjectDisposedException>(async () =>
            {
                await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);
            });

        }

        [Fact]
        public async Task SuccessfulGetRequestAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
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
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

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
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons/100/Details");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulGetRequestWithTemplateGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons/100/Details");
            });

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
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons?Age=18&Name=John Smith");
            });

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
        public async Task SuccessfulGetRequestWithQueryAndTemplateGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons?Name=John Smith");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestWithQueryAsync<MockPersonDto, MockPersonDto>(Method.Get, MockPersonDto.John, dto => new { dto.Name }, "{0}/Friends", 1);

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
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.NotFound, reasonPhrase)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
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
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.NotFound, reasonPhrase)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

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
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .ResponseIsDelayed(10)
                    .RespondsToRequestsWith(Method.Get);
            });

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
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .ResponseIsDelayed(10)
                    .RespondsToRequestsWith(Method.Get);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe("The Request Timed Out; Retry limit reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Result.ShouldBeNull();

            apiHandler.RetryCount.ShouldBe(3);
        }

        [Fact]
        public async Task SuccessfulGetRequestWithTimeoutAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(10, 2)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            apiHandler.RetryCount.ShouldBe(3);
            apiHandler.Timeout.ShouldBe(new TimeSpan(0, 0, 5));
        }

        [Fact]
        public async Task SuccessfulGetRequestWithTimeoutGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .ResponseIsDelayed(10, 2)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.John.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.John.Name);
            personResult.Age.ShouldBe(MockPersonDto.John.Age);

            apiHandler.RetryCount.ShouldBe(3);
        }

        [Fact]
        public async Task UnSuccessfulGetRequestWithTimeoutAsync()
        {
            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.NotFound, reasonPhrase)
                    .ResponseIsDelayed(10, 2)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InPathRequestAsync(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();

            apiHandler.RetryCount.ShouldBe(3);
        }

        [Fact]
        public async Task UnSuccessfulGetRequestWithTimeoutGenericAsync()
        {
            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.NotFound, reasonPhrase)
                    .ResponseIsDelayed(10, 2)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);

            response.WasSuccessful.ShouldBe(false);
            response.Message.ShouldBe(reasonPhrase);
            response.Exception.ShouldBeNull();
            response.Result.ShouldBeNull();

            apiHandler.RetryCount.ShouldBe(3);
            apiHandler.Timeout.ShouldBe(new TimeSpan(0, 0, 5));
        }

        [Fact]
        public async Task SuccessfulPostRequestAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Post)
                    .RespondsToRequestsWith("http://localhost/api/Persons")
                    .RespondsToRequestsWith(MockPersonDto.John);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InBodyRequestAsync(Method.Post, MockPersonDto.John);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulPostRequestGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            }).WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .RespondsToRequestsWith("http://localhost/api/Persons")
                    .RespondsToRequestsWith(MockPersonDto.John);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Post, MockPersonDto.John);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.John.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.John.Name);
            personResult.Age.ShouldBe(MockPersonDto.John.Age);
        }

        [Fact]
        public async Task SuccessfulPostRequestWithTemplateAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Post)
                    .RespondsToRequestsWith("http://localhost/api/Persons/100/Details")
                    .RespondsToRequestsWith(MockPersonDto.John);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InBodyRequestAsync(Method.Post, MockPersonDto.John, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulPostRequestWithTemplateGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterHandlerOptions<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.John)
                    .RespondsToRequestsWith("http://localhost/api/Persons/100/Details")
                    .RespondsToRequestsWith(MockPersonDto.John);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Post, MockPersonDto.John, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.John.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.John.Name);
            personResult.Age.ShouldBe(MockPersonDto.John.Age);
        }
    }

}
