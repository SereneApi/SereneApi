using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Responses;
using SereneApi.Handlers.Rest;
using System;

namespace SereneApi.Extensions.DependencyInjection.Tests
{
    public class TestApiHandler : RestApiHandler, ITestApi
    {
        public new IApiSettings Settings => base.Settings;

        public TestApiHandler(IApiSettings<TestApiHandler> options) : base(options)
        {
        }

        protected override IApiResponse GenerateFailureResponse(IApiRequest request, Status status, string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        protected override IApiResponse<TResponse> GenerateFailureResponse<TResponse>(IApiRequest request, Status status, string message, Exception exception)
        {
            throw new NotImplementedException();
        }
    }

    public interface ITestApi : IApiHandler
    {
    }
}
