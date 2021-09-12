using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Handlers.Soap.Configuration;
using SereneApi.Handlers.Soap.Requests.Factories;
using SereneApi.Handlers.Soap.Responses.Types;
using System;

namespace SereneApi.Handlers.Soap
{
    [UseConfigurationFactory(typeof(SoapConfigurationFactory))]
    public abstract class SoapApiHandler : ApiHandlerBase
    {
        protected IRequestFactory MakeRequest => new SoapRequestFactory(this);

        protected SoapApiHandler(IApiOptions options) : base(options)
        {
        }

        protected override IApiResponse BuildFailureResponse(IApiRequest request, Status status, string message, Exception exception)
        {
            return SoapApiResponse.Failure(request, status, TimeSpan.Zero, message, exception);
        }

        protected override IApiResponse<TResponse> BuildFailureResponse<TResponse>(IApiRequest request, Status status, string message, Exception exception)
        {
            return SoapApiResponse<TResponse>.Failure(request, status, TimeSpan.Zero, message, exception);
        }
    }
}