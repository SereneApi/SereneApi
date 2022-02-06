using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Configuration.Settings;
using SereneApi.Core.Handler;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Responses;
using SereneApi.Handlers.Rest.Configuration;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Responses;
using DeltaWare.Dependencies.Abstractions;
using System;

namespace SereneApi.Handlers.Rest
{
    [UseHandlerConfigurationProvider(typeof(RestHandlerConfigurationProvider))]
    public abstract class RestApiHandler : ApiHandlerBase
    {
        protected IApiRequestMethod MakeRequest
        {
            get
            {
                IApiRequestFactory factory = Settings.Dependencies.GetRequiredDependency<IApiRequestFactory>();

                factory.Handler = this;

                return factory;
            }
        }

        protected RestApiHandler(IApiSettings settings) : base(settings)
        {
        }

        protected override IApiResponse GenerateFailureResponse(IApiRequest request, Status status, string message, Exception exception)
        {
            return RestApiResponse.Failure(request, status, TimeSpan.Zero, message, exception);
        }

        protected override IApiResponse<TResponse> GenerateFailureResponse<TResponse>(IApiRequest request, Status status, string message, Exception exception)
        {
            return RestApiResponse<TResponse>.Failure(request, status, TimeSpan.Zero, message, exception);
        }
    }
}