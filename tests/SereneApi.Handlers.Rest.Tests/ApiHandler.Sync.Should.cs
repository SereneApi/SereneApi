using SereneApi.Core.Handler.Factories;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Core.Responses.Types;
using SereneApi.Extensions.Mocking.Rest;
using SereneApi.Handlers.Rest.Configuration;
using SereneApi.Handlers.Rest.Tests.Interfaces;
using SereneApi.Handlers.Rest.Tests.Mock;
using Shouldly;
using System;
using System.Text.Json;
using Xunit;

namespace SereneApi.Handlers.Rest.Tests
{
    public class ApiHandlerSyncShould
    {
        private readonly RestConfigurationFactory _configuration = new();

        #region Exceptions

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionSerializerResponseGetRequest(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o => o.SetSource(source, resource))
                .EnableRestMocking(c =>
                {
                    c.RegisterMockResponse()
                        .ForMethod(Method.Get)
                        .ForEndpoints(fullSource)
                        .RespondsWith(MockPersonDto.All);
                });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("Could not deserialize returned value.");
            response.Exception.ShouldBeOfType<JsonException>();
            response.Status.ShouldBe(Status.Ok);

            #endregion Assert
        }

        #endregion Exceptions

        #region Basic Get Tests

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:443", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void SuccessfulBasicGetRequest(string source, string resource)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source, resource));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(Status.Ok);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:443", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void SuccessfulBasicGetRequestGeneric(string source, string resource)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source, resource));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(MockPersonDto.JohnSmith);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Data;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 1)]
        public void SuccessfulBasicGetRequestWithTimeout(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(Status.Ok)
                    .IsDelayed(timeoutSeconds + 2, retryCount - 1);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest.UsingMethod(Method.Get).Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 1)]
        public void SuccessfulBasicGetRequestWithTimeoutGeneric(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(MockPersonDto.JohnSmith)
                    .IsDelayed(timeoutSeconds + 2, retryCount - 1);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.Exception.ShouldBeNull();
            response.HasException.ShouldBe(false);
            response.WasSuccessful.ShouldBe(true);
            response.Message.ShouldBeNull();

            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Data;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 0, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 1, 2)]
        public void TimedOutBasicGetRequest(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);

                if (retryCount > 0)
                {
                    o.SetRetryAttempts(retryCount);
                }
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(Status.Ok)
                    .IsDelayed(timeoutSeconds + 2, retryCount);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("The Request Timed Out; The retry limit was reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Status.ShouldBe(Status.TimedOut);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 0, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 1, 2)]
        public void TimedOutBasicGetRequestGeneric(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);

                if (retryCount > 0)
                {
                    o.SetRetryAttempts(retryCount);
                }
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(Status.Ok)
                    .IsDelayed(timeoutSeconds + 2, retryCount);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("The Request Timed Out; The retry limit was reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Status.ShouldBe(Status.TimedOut);
            response.Data.ShouldBeNull();

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:443", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void UnSuccessfulBasicGetRequest(string source, string resource)
        {
            #region Arrange

            const string message = "Exception occurred whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source, resource));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(new FailureResponse(message), status);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:443", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void UnSuccessfulBasicGetRequestGeneric(string source, string resource)
        {
            #region Arrange

            const string message = "Exception occurred whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source, resource));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(new FailureResponse(message), status);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Data.ShouldBeNull();

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 1)]
        public void UnSuccessfulBasicGetRequestWithTimeout(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            const string message = "Exception occurred whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(new FailureResponse(message), status)
                    .IsDelayed(timeoutSeconds + 2, retryCount - 1);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 1)]
        public void UnSuccessfulBasicGetRequestWithTimeoutGeneric(string source, string resource, int retryCount, int timeoutSeconds)
        {
            #region Arrange

            const string message = "Exception occurred whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(finalSource)
                    .RespondsWith(new FailureResponse(message), status)
                    .IsDelayed(timeoutSeconds + 2, retryCount - 1);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Data.ShouldBeNull();

            #endregion Assert
        }

        #endregion Basic Get Tests

        #region Get Against Resource Tests

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:443", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void SuccessfulBasicGetRequestAgainstResourceGeneric(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";
            string finalSource = $"{source}/api";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(fullSource)
                    .RespondsWith(MockPersonDto.JohnSmith);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstResource(resource)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBeNull();
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Data;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com", "path/resource")]
        [InlineData("http://test.source.com", "path/path/resource")]
        [InlineData("http://test.source.com:443", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void SuccessfulGetRequestAgainstResource(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";
            string finalSource = $"{source}/api";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(fullSource)
                    .RespondsWith(Status.Ok);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstResource(resource)
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBeNull();
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            #endregion Assert
        }

        #endregion Get Against Resource Tests

        #region Get With Endpoint Tests

        [Theory]
        [InlineData("http://test.source.com", "resource", "endpoint")]
        [InlineData("http://test.source.com", "path/resource", "endpoint/endpoint")]
        [InlineData("http://test.source.com", "path/path/resource", "endpoint/endpoint/endpoint")]
        [InlineData("http://test.source.com:443", "test/resource", "endpoint")]
        [InlineData("http://test.source.com:443", "path/resource", "endpoint/endpoint")]
        [InlineData("http://test.source.com:8080", "path/path/resource", "endpoint/endpoint/endpoint")]
        public void SuccessfulGetRequestWithEndpoint(string source, string resource, string endpoint)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}/{endpoint}";
            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source, resource));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(fullSource)
                    .RespondsWith(Status.Ok);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint(endpoint)
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            #endregion Assert
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", "endpoint")]
        [InlineData("http://test.source.com", "path/resource", "endpoint/endpoint")]
        [InlineData("http://test.source.com", "path/path/resource", "endpoint/endpoint/endpoint")]
        [InlineData("http://test.source.com:443", "test/resource", "endpoint")]
        [InlineData("http://test.source.com:443", "path/resource", "endpoint/endpoint")]
        [InlineData("http://test.source.com:8080", "path/path/resource", "endpoint/endpoint/endpoint")]
        public void SuccessfulGetRequestWithEndpointGeneric(string source, string resource, string endpoint)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}/{endpoint}";
            string finalSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                o => o.SetSource(source, resource));

            apiFactory.ExtendApi<IApiHandlerWrapper>().EnableRestMocking(c =>
            {
                c.RegisterMockResponse()
                    .ForMethod(Method.Get)
                    .ForEndpoints(fullSource)
                    .RespondsWith(MockPersonDto.JohnSmith);
            });

            #endregion Arrange

            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint(endpoint)
                .RespondsWith<MockPersonDto>()
                .Execute());

            #endregion Act

            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.Configuration["ResourcePath"]);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.Configuration.Get<int>("RetryAttempts"));
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Configuration.Get<int>("Timeout"));

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Data;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion Assert
        }

        #endregion Get With Endpoint Tests
    }
}