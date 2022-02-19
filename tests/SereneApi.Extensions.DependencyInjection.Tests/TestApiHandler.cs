using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Responses;
using System;
using SereneApi.Core.Configuration.Attributes;
using SereneApi.Handlers.Rest.Configuration;

namespace SereneApi.Extensions.DependencyInjection.Tests
{
    [UseHandlerConfigurationProvider(typeof(RestHandlerConfigurationProvider))]
    public class TestApiHandler : ApiHandlerBase, ITestApi
    {
        public new IApiSettings Settings => base.Settings;

        public TestApiHandler(IApiSettings<TestApiHandler> settings) : base(settings)
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
}