using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Response;
using SereneApi.Extensions.Mocking;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Tests.Interfaces;
using SereneApi.Tests.Mock;
using Shouldly;
using System;
using System.Text.Json;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerSyncShould
    {
        #region Exceptions

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionSerializerResponseGetRequest(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource))
            .WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.All)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("Could not deserialize returned value.");
            response.Exception.ShouldBeOfType<JsonException>();
            response.Status.ShouldBe(Status.Ok);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionGetRequestAgainstResourceWhenResourceAssigned(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest(Method.GET, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionGetRequestAgainstResourceWhenResourceAssignedGeneric(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionDeleteRequestAgainstResourceWhenResourceAssigned(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest(Method.DELETE, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionDeleteRequestAgainstResourceWhenResourceAssignedGeneric(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.DELETE, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionPostRequestAgainstResourceWhenResourceAssigned(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest(Method.POST, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionPostRequestAgainstResourceWhenResourceAssignedGeneric(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.POST, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }


        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionPutRequestAgainstResourceWhenResourceAssigned(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest(Method.PUT, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionPutRequestAgainstResourceWhenResourceAssignedGeneric(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.PUT, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionPatchRequestAgainstResourceWhenResourceAssigned(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest(Method.PATCH, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

            #endregion
        }

        [Theory]
        [InlineData("http://test.source.com", "resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void ExceptionPatchRequestAgainstResourceWhenResourceAssignedGeneric(string source, string resource)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            #endregion
            #region Assert

            Should.Throw<MemberAccessException>(() =>
            {
                apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.PATCH, r => r.AgainstResource(resource));
            });

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(fullSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
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

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

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
        [InlineData("http://test.source.com:443", "test/resource")]
        [InlineData("http://test.source.com:443", "path/resource")]
        [InlineData("http://test.source.com:8080", "path/path/resource")]
        public void UnSuccessfulBasicGetRequest(string source, string resource)
        {
            #region Arrange

            const string message = "Exception occured whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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

            const string message = "Exception occured whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
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

            const string message = "Exception occured whilst getting.";
            const Status status = Status.InternalServerError;

            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeout(timeoutSeconds);
                o.SetRetryAttempts(retryCount);
            });

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(status, message)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount - 1)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

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

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeout(timeoutSeconds);

                if(retryCount > 0)
                {
                    o.SetRetryAttempts(retryCount);
                }
            });

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("The Request Timed Out; Retry limit reached");
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

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(o =>
            {
                o.UseSource(source, resource);
                o.SetTimeout(timeoutSeconds);

                if(retryCount > 0)
                {
                    o.SetRetryAttempts(retryCount);
                }
            });

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .ResponseIsDelayed(timeoutSeconds + 2, retryCount)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(finalSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(retryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(timeoutSeconds);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Message.ShouldBe("The Request Timed Out; Retry limit reached");
            response.Exception.ShouldBeOfType<TimeoutException>();
            response.Status.ShouldBe(Status.None);
            response.Result.ShouldBeNull();

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
            string finalSource = $"{source}/api/";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest(Method.GET, r => r.AgainstResource(resource)));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBeNull();
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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
            string finalSource = $"{source}/api/";


            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET, r => r.AgainstResource(resource)));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBeNull();
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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

        #endregion
        #region Get With EndPoint Tests

        [Theory]
        [InlineData("http://test.source.com", "resource", "endpoint")]
        [InlineData("http://test.source.com", "path/resource", "endpoint/endpoint")]
        [InlineData("http://test.source.com", "path/path/resource", "endpoint/endpoint/endpoint")]
        [InlineData("http://test.source.com:443", "test/resource", "endpoint")]
        [InlineData("http://test.source.com:443", "path/resource", "endpoint/endpoint")]
        [InlineData("http://test.source.com:8080", "path/path/resource", "endpoint/endpoint/endpoint")]
        public void SuccessfulGetRequestWithEndPoint(string source, string resource, string endPoint)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}/{endPoint}";
            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(Status.Ok)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest(Method.GET, r => r.WithEndPoint(endPoint)));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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
        public void SuccessfulGetRequestWithEndPointGeneric(string source, string resource, string endPoint)
        {
            #region Arrange

            string fullSource = $"{source}/api/{resource}/{endPoint}";
            string finalSource = $"{source}/api/{resource}";

            using ApiHandlerFactory handlerFactory = new ApiHandlerFactory();

            handlerFactory.RegisterApiHandler<IApiHandlerWrapper, ApiHandlerWrapper>(
                o => o.UseSource(source, resource));

            handlerFactory.ExtendApi<IApiHandlerWrapper>().WithMockResponses(r =>
            {
                r.AddMockResponse(MockPersonDto.JohnSmith)
                    .RespondsToRequestsWith(Method.GET)
                    .RespondsToRequestsWith(fullSource);
            });

            #endregion
            #region Act

            using IApiHandlerWrapper apiHandlerWrapper = Should.NotThrow(() => handlerFactory.Build<IApiHandlerWrapper>());

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandlerWrapper.PerformRequest<MockPersonDto>(Method.GET, r => r.WithEndPoint(endPoint)));

            #endregion
            #region Assert

            apiHandlerWrapper.Connection.Resource.ShouldBe(resource);
            apiHandlerWrapper.Connection.Source.ShouldBe(finalSource);
            apiHandlerWrapper.Connection.ResourcePath.ShouldBe(SereneApiConfiguration.Default.ResourcePath);
            apiHandlerWrapper.Connection.RetryAttempts.ShouldBe(SereneApiConfiguration.Default.RetryCount);
            apiHandlerWrapper.Connection.Timeout.ShouldBe(SereneApiConfiguration.Default.Timeout);

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

        #endregion
    }

}
