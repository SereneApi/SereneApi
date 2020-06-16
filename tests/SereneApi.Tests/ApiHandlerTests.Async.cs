using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking;
using SereneApi.Factories;
using SereneApi.Tests.Mock;
using Shouldly;
using System;
using System.Threading.Tasks;
using SereneApi.Helpers;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerTestsAsync
    {
        #region Basic Get Tests

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:80", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]

        public void SuccessfulBasicGetRequest(string source, string resource)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(ApiHandlerOptionDefaults.RetryCount);
            apiHandler.Timeout.ShouldBe(ApiHandlerOptionDefaults.TimeoutPeriod);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:80", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]

        public void SuccessfulBasicGetRequestGeneric(string source, string resource)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync<MockPersonDto>(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(ApiHandlerOptionDefaults.RetryCount);
            apiHandler.Timeout.ShouldBe(ApiHandlerOptionDefaults.TimeoutPeriod);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Result;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 2)]

        public void SuccessfulBasicGetRequestWithTimeout(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeoutPeriod(timeoutSeconds);
                o.SetRetryOnTimeout(retryCount);
            });

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(retryCount);
            apiHandler.Timeout.ShouldBe(TimeSpan.FromSeconds(timeoutSeconds));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 2)]

        public void SuccessfulBasicGetRequestWithTimeoutGeneric(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeoutPeriod(timeoutSeconds);
                o.SetRetryOnTimeout(retryCount);
            });

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync<MockPersonDto>(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(retryCount);
            apiHandler.Timeout.ShouldBe(TimeSpan.FromSeconds(timeoutSeconds));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Result;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:80", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]

        public void UnSuccessfulBasicGetRequest(string source, string resource)
        {
            #region Arrange

            const string message = "Exception occured whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(ApiHandlerOptionDefaults.RetryCount);
            apiHandler.Timeout.ShouldBe(ApiHandlerOptionDefaults.TimeoutPeriod);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:80", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]

        public void UnSuccessfulBasicGetRequestGeneric(string source, string resource)
        {
            #region Arrange

            const string message = "Exception occured whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync<MockPersonDto>(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(ApiHandlerOptionDefaults.RetryCount);
            apiHandler.Timeout.ShouldBe(ApiHandlerOptionDefaults.TimeoutPeriod);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Result.ShouldBeNull();

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 2)]

        public void UnSuccessfulBasicGetRequestWithTimeout(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            const string message = "Exception occured whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeoutPeriod(timeoutSeconds);
                o.SetRetryOnTimeout(retryCount);
            });

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(retryCount);
            apiHandler.Timeout.ShouldBe(TimeSpan.FromSeconds(timeoutSeconds));

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 2)]

        public void UnSuccessfulBasicGetRequestWithTimeoutGeneric(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            const string message = "Exception occured whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeoutPeriod(timeoutSeconds);
                o.SetRetryOnTimeout(retryCount);
            });

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync<MockPersonDto>(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(retryCount);
            apiHandler.Timeout.ShouldBe(TimeSpan.FromSeconds(timeoutSeconds));

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Result.ShouldBeNull();

            #endregion
        }


        [Theory]
        [InlineData("http://test.source.com", "resource", 0, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 1, 2)]

        public void TimedOutBasicGetRequest(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeoutPeriod(timeoutSeconds);

                if(retryCount > 0)
                {
                    o.SetRetryOnTimeout(retryCount);
                }
            });

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(retryCount);
            apiHandler.Timeout.ShouldBe(TimeSpan.FromSeconds(timeoutSeconds));

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe(MessageHelper.RequestTimedOutRetryLimit);
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Status.ShouldBe(Status.None);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 0, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 1, 2)]

        public void TimedOutBasicGetRequestGeneric(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<TestApiHandler>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeoutPeriod(timeoutSeconds);

                if(retryCount > 0)
                {
                    o.SetRetryOnTimeout(retryCount);
                }
            });

            handlerFactory.ExtendApiHandler<TestApiHandler>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using TestApiHandler apiHandler = Should.NotThrow(() => handlerFactory.Build<TestApiHandler>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandler.PerformRequestAsync<MockPersonDto>(Method.Get));

            #endregion
            #region Assert

            apiHandler.Resource.ShouldBe(resource);
            apiHandler.Source.ShouldBe(new Uri(finalSource));
            apiHandler.ResourcePath.ShouldBe(ApiHandlerOptionDefaults.ResourcePath);
            apiHandler.RetryCount.ShouldBe(retryCount);
            apiHandler.Timeout.ShouldBe(TimeSpan.FromSeconds(timeoutSeconds));

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe(MessageHelper.RequestTimedOutRetryLimit);
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Status.ShouldBe(Status.None);
            response.Result.ShouldBeNull();

            #endregion
        }

        #endregion





        #region Legacy

        [Fact]
        public void GetRequestWithInvalidTemplateA()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync(Method.Get, MockPersonDto.JohnSmith);
            });
        }

        [Fact]
        public void ExceptionInBodyGetGeneric()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Get, MockPersonDto.JohnSmith);
            });
        }

        [Fact]
        public void ExceptionInBodyDelete()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync(Method.Delete, MockPersonDto.JohnSmith);
            });
        }

        [Fact]
        public void ExceptionInBodyDeleteGeneric()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            Should.Throw<ArgumentException>(async () =>
            {
                await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Delete, MockPersonDto.JohnSmith);
            });
        }

        [Fact]
        public void ExceptionDisposedAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            apiHandler.Dispose();

            Should.Throw<ObjectDisposedException>(async () =>
            {
                await apiHandler.InPathRequestAsync(Method.Get);
            });
        }

        [Fact]
        public void ExceptionDisposedGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            apiHandler.Dispose();

            Should.Throw<ObjectDisposedException>(async () =>
            {
                await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get);
            });

        }

        [Fact]
        public async Task SuccessfulGetRequestAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons/0");
                r.AddMockResponse(MockPersonDto.BenJerry)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons/1");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get, 0);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
            personResult.Age.ShouldBe(MockPersonDto.JohnSmith.Age);

            response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get, 1);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.BenJerry.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.BenJerry.Name);
            personResult.Age.ShouldBe(MockPersonDto.BenJerry.Age);
        }

        [Fact]
        public async Task SuccessfulGetRequestWithTemplateAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons/100/Details");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestAsync<MockPersonDto>(Method.Get, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
            personResult.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
        }

        [Fact]
        public async Task SuccessfulGetRequestWithQueryGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons?Age=18&Name=John Smith");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestWithQueryAsync<MockPersonDto, MockPersonDto>(Method.Get, MockPersonDto.JohnSmith, dto => new { dto.Age, dto.Name });

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
            personResult.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
        }

        [Fact]
        public async Task SuccessfulGetRequestWithQueryAndTemplateGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith("http://localhost/api/Persons/1/Friends?Name=John Smith");
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InPathRequestWithQueryAsync<MockPersonDto, MockPersonDto>(Method.Get, MockPersonDto.JohnSmith, dto => new { dto.Name }, "{0}/Friends", 1);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
            personResult.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
        }

        [Fact]
        public async Task UnSuccessfulGetRequestAsync()
        {
            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
                builder.SetRetryOnTimeout(3);
                builder.SetTimeoutPeriod(5);
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
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

            personResult.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
            personResult.Age.ShouldBe(MockPersonDto.JohnSmith.Age);

            apiHandler.RetryCount.ShouldBe(3);
        }

        [Fact]
        public async Task UnSuccessfulGetRequestWithTimeoutAsync()
        {
            const string reasonPhrase = "Could not find the specified person";

            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
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

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Post)
                    .RespondsToRequestsWith("http://localhost/api/Persons")
                    .RespondsToRequestsWith(MockPersonDto.JohnSmith);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InBodyRequestAsync(Method.Post, MockPersonDto.JohnSmith);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulPostRequestGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            }).WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith("http://localhost/api/Persons")
                    .RespondsToRequestsWith(MockPersonDto.JohnSmith);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Post, MockPersonDto.JohnSmith);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
            personResult.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
        }

        [Fact]
        public async Task SuccessfulPostRequestWithTemplateAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Post)
                    .RespondsToRequestsWith("http://localhost/api/Persons/100/Details")
                    .RespondsToRequestsWith(MockPersonDto.JohnSmith);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse response = await apiHandler.InBodyRequestAsync(Method.Post, MockPersonDto.JohnSmith, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
        }

        [Fact]
        public async Task SuccessfulPostRequestWithTemplateGenericAsync()
        {
            using ApiHandlerFactory factory = new ApiHandlerFactory();

            factory.RegisterApiHandler<TestApiHandler>(builder =>
            {
                builder.UseSource("http://localhost", "Persons");
            })
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith("http://localhost/api/Persons/100/Details")
                    .RespondsToRequestsWith(MockPersonDto.JohnSmith);
            });

            using TestApiHandler apiHandler = factory.Build<TestApiHandler>();

            IApiResponse<MockPersonDto> response = await apiHandler.InBodyRequestAsync<MockPersonDto, MockPersonDto>(Method.Post, MockPersonDto.JohnSmith, "{0}/Details", 100);

            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();

            MockPersonDto personResult = response.Result;

            personResult.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            personResult.Name.ShouldBe(MockPersonDto.JohnSmith.Name);
            personResult.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
        }

        #endregion
    }
}
