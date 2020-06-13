using SereneApi.Abstraction.Enums;
using SereneApi.Types;
using Shouldly;
using System;
using Xunit;

namespace SereneApi.Tests.Types
{
    public class ApiResponseTests
    {
        [Fact]
        public void ApiHandlerSuccess()
        {
            Status status = Status.Ok;

            IApiResponse response = ApiResponse.Success(status);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBeNull();
            response.Status.ShouldBe(status);
        }

        [Fact]
        public void ApiHandlerGenericSuccess()
        {
            string resultString = "Success!";
            Status status = Status.Ok;

            IApiResponse<string> response = ApiResponse<string>.Success(status, resultString);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Result.ShouldBe(resultString);
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("Something went wrong")]
        [InlineData("This is a test message.")]
        public void ApiHandlerFailureMessage(string message)
        {
            Status status = Status.InternalServerError;

            IApiResponse response = ApiResponse.Failure(status, message);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Status.ShouldBe(status);
            response.Message.ShouldBe(message);
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("Something went wrong")]
        [InlineData("This is a test message.")]
        public void ApiHandlerGenericFailureMessage(string message)
        {
            Status status = Status.InternalServerError;

            IApiResponse<string> response = ApiResponse<string>.Failure(status, message);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBe(message);
            response.Status.ShouldBe(status);
            response.Result.ShouldBeNull();
        }

        [Fact]
        public void ApiHandlerFailureException()
        {
            string message = "An Exception Happened.";
            ArgumentException argumentException = new ArgumentException("Bad params man");
            Status status = Status.InternalServerError;

            IApiResponse response = ApiResponse.Failure(status, message, argumentException);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Exception.ShouldBe(argumentException);
            response.Status.ShouldBe(status);
            response.Message.ShouldBe(message);
        }

        [Fact]
        public void ApiHandlerGenericFailureException()
        {
            string message = "An Exception Happened.";
            ArgumentException argumentException = new ArgumentException("Bad params man");
            Status status = Status.InternalServerError;

            IApiResponse<string> response = ApiResponse<string>.Failure(status, message, argumentException);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Exception.ShouldBe(argumentException);
            response.Message.ShouldBe(message);
            response.Status.ShouldBe(status);
            response.Result.ShouldBeNull();
        }
    }
}
