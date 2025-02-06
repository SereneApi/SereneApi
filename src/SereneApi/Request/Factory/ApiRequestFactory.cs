using DeltaWare.SDK.SmartFormat;
using SereneApi.Resource.Schema;
using SereneApi.Resource.Schema.Enums;
using System.Collections.Generic;
using System.Threading;

namespace SereneApi.Request.Factory
{
    internal sealed class ApiRequestFactory
    {
        private readonly ApiResourceConnection _connection;

        public ApiRequestFactory(ApiResourceConnection connection)
        {
            _connection = connection;
        }

        public IApiRequest Build(ApiRouteSchema routeSchema, IReadOnlyList<object> parameters)
        {
            ApiRequest request = new ApiRequest(routeSchema.Method);

            request.Version = request.Version;
            request.Route = routeSchema.GetRoute(parameters);
            request.Query = routeSchema.GetQuery(parameters);
            request.Content = routeSchema.GetParameter(parameters, ApiRouteParameterType.Content);
            request.Headers = routeSchema.GetHeaders(parameters);
            request.CancellationToken = routeSchema.GetParameter<CancellationToken>(parameters, ApiRouteParameterType.CancellationToken);
            request.ResponseType = routeSchema.Response?.ResponseType;
            request.FullRoute = SmartFormat.Parse(_connection.UrlTemplate, new
            {
                Host = _connection.HostUrl,
                Resource = routeSchema.ParentResource.Name,
                request.Version,
                request.Route,
                request.Query,
            });

            return request;
        }
    }
}
