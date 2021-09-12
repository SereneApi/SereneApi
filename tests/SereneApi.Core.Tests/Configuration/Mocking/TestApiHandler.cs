using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using System;

namespace SereneApi.Core.Tests.Configuration.Mocking
{
    [UseConfigurationFactory(typeof(TestConfigurationFactory))]
    public class TestApiHandler : ApiHandlerBase
    {
        public TestApiHandler(IApiOptions<TestApiHandler> options) : base(options)
        {
        }

        protected override IApiResponse BuildFailureResponse(IApiRequest request, Status status, string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        protected override IApiResponse<TResponse> BuildFailureResponse<TResponse>(IApiRequest request, Status status, string message,
            Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}