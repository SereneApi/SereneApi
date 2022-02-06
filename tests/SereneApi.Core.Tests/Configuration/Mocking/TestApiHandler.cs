using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Responses;
using System;

namespace SereneApi.Core.Tests.Configuration.Mocking
{
    [UseHandlerConfigurationProvider(typeof(TestHandlerConfigurationProvider))]
    public class TestApiHandler : ApiHandlerBase
    {
        public TestApiHandler(IApiSettings<TestApiHandler> settings) : base(settings)
        {
        }

        protected override IApiResponse GenerateFailureResponse(IApiRequest request, Status status, string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        protected override IApiResponse<TResponse> GenerateFailureResponse<TResponse>(IApiRequest request, Status status, string message,
            Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}