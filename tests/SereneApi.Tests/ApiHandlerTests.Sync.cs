using System;
using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking;
using SereneApi.Factories;
using SereneApi.Helpers;
using SereneApi.Tests.Mock;
using Shouldly;
using Xunit;

namespace SereneApi.Tests
{
    public class ApiHandlerTestsSync
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

            IApiResponse response = Should.NotThrow(() => apiHandler.PerformRequest(Method.Get));

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

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandler.PerformRequest<MockPersonDto>(Method.Get));

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

            IApiResponse response = Should.NotThrow(() => apiHandler.PerformRequest(Method.Get));

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

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandler.PerformRequest<MockPersonDto>(Method.Get));

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

            IApiResponse response = Should.NotThrow(() => apiHandler.PerformRequest(Method.Get));

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

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandler.PerformRequest<MockPersonDto>(Method.Get));

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

            IApiResponse response = Should.NotThrow(() => apiHandler.PerformRequest(Method.Get));

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

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandler.PerformRequest<MockPersonDto>(Method.Get));

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

                if (retryCount > 0)
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

            IApiResponse response = Should.NotThrow(() => apiHandler.PerformRequest(Method.Get));

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

            IApiResponse<MockPersonDto> response = Should.NotThrow(() => apiHandler.PerformRequest<MockPersonDto>(Method.Get));

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
    }

}
