using DeltaWare.SDK.SmartFormat;
using SereneApi.Helpers;
using SereneApi.Resource.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Request.Factory
{
    internal sealed class ApiRequestFactory
    {
        private readonly IApiResourceConnection _connection;

        public ApiRequestFactory(IApiResourceConnection connection)
        {
            _connection = connection;
        }

        public IApiRequest Build(ApiRouteSchema routeSchema, object[] parameters)
        {
            ApiRequest request = new ApiRequest(routeSchema.Method);

            request.Version = request.Version;
            request.Route = BuildRoute(routeSchema, parameters);
            request.Query = BuildQuery(routeSchema, parameters);
            request.Content = GetContent(routeSchema, parameters);
            request.Headers = BuildHeaders(routeSchema, parameters);
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

        private static string? BuildRoute(ApiRouteSchema routeSchema, object[] parameters)
        {
            var routeParameters = routeSchema
                .GetRouteParameterSchemas()
                .ToArray();

            if (routeParameters.Length == 0)
            {
                return routeSchema.Template;
            }

            if (routeSchema.Template is null)
            {
                throw new InvalidOperationException("Route template cannot be null.");
            }

            var matchedParameters = routeParameters
                .OrderBy(p => p.TemplateIndex)
                .Select(p => parameters[p.ParameterIndex])
                .ToArray();

            return string.Format(routeSchema.Template, matchedParameters);
        }

        private static string BuildQuery(ApiRouteSchema routeSchema, object[] parameters)
        {
            Dictionary<string, string> querySections = routeSchema
                .GetQuerySchemas()
                .ToDictionary(queryParameter => queryParameter.Name, queryParameter => parameters[queryParameter.ParameterIndex].ToString());

            return QueryHelper.BuildQueryString(querySections);
        }

        private static object? GetContent(ApiRouteSchema routeSchema, object[] parameters)
        {
            ApiRouteParameterSchema? contentSchema = routeSchema.GetContentSchema();

            if (contentSchema == null)
            {
                return null;
            }

            return parameters[contentSchema.Value.ParameterIndex];
        }

        private static IReadOnlyDictionary<string, string> BuildHeaders(ApiRouteSchema routeSchema, object[] parameters)
        {
            Dictionary<string, string> headers = routeSchema.Headers
                .ToDictionary(k => k.Key, v => v.Value);

            foreach (ApiRouteParameterSchema headerParameter in routeSchema.GetHeaderSchemas())
            {
                headers.Add(headerParameter.Name, parameters[headerParameter.ParameterIndex].ToString());
            }

            return headers;
        }
    }
}
