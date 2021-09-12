using SereneApi.Core.Configuration.Attributes;
using SereneApi.Core.Handler;
using SereneApi.Core.Options;
using SereneApi.Core.Requests;
using SereneApi.Core.Responses;
using SereneApi.Handlers.Rest.Configuration;
using SereneApi.Handlers.Rest.Requests.Factories;
using SereneApi.Handlers.Rest.Responses.Types;
using System;

namespace SereneApi.Handlers.Rest
{
    [UseConfigurationFactory(typeof(RestConfigurationFactory))]
    public abstract class RestApiHandler : ApiHandlerBase
    {
        protected IApiRequestMethod MakeRequest
        {
            get
            {
                IApiRequestFactory factory = Options.Dependencies.GetDependency<IApiRequestFactory>();

                factory.Handler = this;

                return factory;
            }
        }

        protected RestApiHandler(IApiOptions options) : base(options)
        {
        }

        protected override IApiResponse BuildFailureResponse(IApiRequest request, Status status, string message, Exception exception)
        {
            return RestApiResponse.Failure(request, status, TimeSpan.Zero, message, exception);
        }

        protected override IApiResponse<TResponse> BuildFailureResponse<TResponse>(IApiRequest request, Status status, string message, Exception exception)
        {
            return RestApiResponse<TResponse>.Failure(request, status, TimeSpan.Zero, message, exception);
        }
    }
}