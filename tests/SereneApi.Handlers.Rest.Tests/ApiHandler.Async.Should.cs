﻿using SereneApi.Core.Handler.Factories;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Extensions.Mocking;
using SereneApi.Handlers.Rest.Configuration;
using SereneApi.Handlers.Rest.Responses.Types;
using SereneApi.Tests.Interfaces;
using SereneApi.Tests.Mock;
using Shouldly;
using System;
using System.Text.Json;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerAsyncShould
    {
        private readonly RestConfigurationProvider _configuration = new();

        #region Exceptions

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionSerializerResponseGetRequest(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(
                    o => o.SetSource(source, resource))
                .WithMockResponse(r =>
                {
                    r.AddMockResponse(MockPersonDto.All)
                        .RespondsToRequestsWith(Method.Get)
                        .RespondsToRequestsWith(fullSource);
                });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("Could not deserialize returned value.");
            response.Exception.ShouldBeOfType<JsonException>();
            response.Status.ShouldBe(Status.Ok);

            #endregion
        }

        #endregion
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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Data;

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

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest.UsingMethod(Method.Get).ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

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

            using ApiFactory apiFactory = new ApiFactory();

            apiFactory.RegisterApi<IApiHandlerWrapper, BaseApiHandlerWrapper>(o =>
            {
                o.SetSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
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

            #endregion
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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(new FailureResponse(message), status)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(new FailureResponse(message), status)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Data.ShouldBeNull();

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource", 2, 2)]
        [InlineData("http://test.source.com:8080", "path/path/resource", 3, 2)]
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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(new FailureResponse(message), status)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(new FailureResponse(message), status)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBe(message);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Data.ShouldBeNull();

            #endregion
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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("The Request Timed Out; The retry limit was reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Status.ShouldBe(Status.TimedOut);

            #endregion
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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("The Request Timed Out; The retry limit was reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Status.ShouldBe(Status.TimedOut);
            response.Data.ShouldBeNull();

            #endregion
        }

        #endregion
        #region Get Against Resource Tests

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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstResource(resource)
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBeNull();
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstResource(resource)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBeNull();
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Data;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion
        }

        #endregion
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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint(endpoint)
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            #endregion
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

            apiFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponse(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.Get)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => apiFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(async () => await apiHandlerWrapper.MakeRequest
                .UsingMethod(Method.Get)
                .AgainstEndpoint(endpoint)
                .RespondsWithType<MockPersonDto>()
                .ExecuteAsync());

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(_configuration.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(_configuration.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(_configuration.Timeout);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Message.ShouldBeNull();
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(Status.Ok);

            MockPersonDto person = response.Data;

            person.Age.ShouldBe(MockPersonDto.JohnSmith.Age);
            person.BirthDate.ShouldBe(MockPersonDto.JohnSmith.BirthDate);
            person.Name.ShouldBe(MockPersonDto.JohnSmith.Name);

            #endregion
        }

        #endregion
    }
}
