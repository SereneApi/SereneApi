using SereneApi.Abstraction;
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
            IApiResponse response = ApiResponse.Success();

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBe(string.Empty);
        }

        [Fact]
        public void ApiHandlerGenericSuccess()
        {
            string resultString = "Success!";

            IApiResponse<string> response = ApiResponse<string>.Success(resultString);

            response.WasSuccessful.ShouldBe(true);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBe(string.Empty);
            response.Result.ShouldBe(resultString);
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("Something went wrong")]
        [InlineData("This is a test message.")]
        public void ApiHandlerFailureMessage(string message)
        {
            IApiResponse response = ApiResponse.Failure(message);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBe(message);
        }

        [Theory]
        [InlineData("Message 1")]
        [InlineData("Something went wrong")]
        [InlineData("This is a test message.")]
        public void ApiHandlerGenericFailureMessage(string message)
        {
            IApiResponse<string> response = ApiResponse<string>.Failure(message);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(false);
            response.Exception.ShouldBeNull();
            response.Message.ShouldBe(message);
            response.Result.ShouldBe(string.Empty);
        }

        [Fact]
        public void ApiHandlerFailureException()
        {
            string message = "An Exception Happened.";
            ArgumentException argumentException = new ArgumentException("Bad params man");

            IApiResponse response = ApiResponse.Failure(message, argumentException);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Exception.ShouldBe(argumentException);
            response.Message.ShouldBe(message);
        }

        [Fact]
        public void ApiHandlerGenericFailureException()
        {
            string message = "An Exception Happened.";
            ArgumentException argumentException = new ArgumentException("Bad params man");

            IApiResponse<string> response = ApiResponse<string>.Failure(message, argumentException);

            response.WasSuccessful.ShouldBe(false);
            response.HasException.ShouldBe(true);
            response.Exception.ShouldBe(argumentException);
            response.Message.ShouldBe(message);
            response.Result.ShouldBe(string.Empty);
        }
    }
}
