using SereneApi.Core.Http.Responses;
using SereneApi.Handlers.Rest.Requests;
using SereneApi.Handlers.Rest.Responses;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Handlers.Rest.Tests.Types
{
    public class ApiResponseTests
    {
        [Fact]
        public void ApiHandlerFailureException()
        {
            string message = "An Exception Happened.";
            ArgumentException argumentException = new ArgumentException("Bad params man");
            Status status = Status.InternalServerError;

            IApiResponse response = RestApiResponse.Failure(RestApiRequest.Empty, status, TimeSpan.Zero, message, argumentException);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Exception.ShouldBe(argumentException);
            response.Status.ShouldBe(status);
            response.Message.ShouldBe(message);
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("Something went wrong")]
        [InlineData("This is a test message.")]
        public void ApiHandlerFailureMessage(string message)
        {
            Status status = Status.InternalServerError;

            IApiResponse response = RestApiResponse.Failure(RestApiRequest.Empty, status, TimeSpan.Zero, message);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Message.ShouldBe(message);
        }

        [Fact]
        public void ApiHandlerGenericFailureException()
        {
            string message = "An Exception Happened.";
            ArgumentException argumentException = new ArgumentException("Bad params man");
            Status status = Status.InternalServerError;

            IApiResponse<string> response = RestApiResponse<string>.Failure(RestApiRequest.Empty, status, TimeSpan.Zero, message, argumentException);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Exception.ShouldBe(argumentException);
            response.Message.ShouldBe(message);
            response.Status.ShouldBe(status);
            response.Data.ShouldBeNull();
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("Something went wrong")]
        [InlineData("This is a test message.")]
        public void ApiHandlerGenericFailureMessage(string message)
        {
            Status status = Status.InternalServerError;

            IApiResponse<string> response = RestApiResponse<string>.Failure(RestApiRequest.Empty, status, TimeSpan.Zero, message);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBe(message);
            response.Status.ShouldBe(status);
            response.Data.ShouldBeNull();
        }

        [Fact]
        public void ApiHandlerGenericSuccess()
        {
            string resultString = "Success!";
            Status status = Status.Ok;

            IApiResponse<string> response = RestApiResponse<string>.Success(RestApiRequest.Empty, status, TimeSpan.Zero, resultString);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Data.ShouldBe(resultString);
        }

        [Fact]
        public void ApiHandlerSuccess()
        {
            Status status = Status.Ok;

            IApiResponse response = RestApiResponse.Success(RestApiRequest.Empty, status, TimeSpan.Zero);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBeNull();
            response.Status.ShouldBe(status);
        }
    }
}