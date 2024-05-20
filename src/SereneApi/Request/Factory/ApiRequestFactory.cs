using SereneApi.Helpers;
using SereneApi.Resource.Schema;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Request.Factory
{
    internal sealed class ApiRequestFactory
    {
        public IApiRequest Build(ApiRouteSchema routeSchema, object[] parameters)
        {
            ApiRequest request = new ApiRequest(routeSchema.Method);

            request.Version = request.Version;
            request.Route = BuildRoute(routeSchema, parameters);
            request.Query = BuildQuery(routeSchema, parameters);
            request.Content = GetContent(routeSchema, parameters);

            foreach (KeyValuePair<string, string> header in BuildHeaders(routeSchema, parameters))
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        private static string? BuildRoute(ApiRouteSchema routeSchema, object[] parameters)
        {
            ApiRouteParameterSchema[] routeParameters = routeSchema
                .GetRouteParameterSchemas()
                .OrderBy(p => p.TemplateIndex)
                .ToArray();

            if (!routeParameters.Any())
            {
                return routeSchema.Template;
            }

            object[] matchedParameters = new object[routeParameters.Length];

            for (int i = 0; i < routeParameters.Length; i++)
            {
                matchedParameters[i] = parameters[routeParameters[i].ParameterIndex];
            }

            return string.Format(routeSchema.Template!, matchedParameters);
        }

        private static string BuildQuery(ApiRouteSchema routeSchema, object[] parameters)
        {
            Dictionary<string, string> querySections = new Dictionary<string, string>();

            foreach (ApiRouteParameterSchema? queryParameter in routeSchema.GetQuerySchemas())
            {
                querySections.Add(queryParameter.Name, parameters[queryParameter.ParameterIndex].ToString());
            }

            return QueryHelper.BuildQueryString(querySections);
        }

        private static object? GetContent(ApiRouteSchema routeSchema, object[] parameters)
        {
            ApiRouteParameterSchema? contentSchema = routeSchema.GetContentSchema();

            if (contentSchema == null)
            {
                return null;
            }

            return parameters[contentSchema.ParameterIndex];
        }

        private static IReadOnlyDictionary<string, string> BuildHeaders(ApiRouteSchema routeSchema, object[] parameters)
        {
            Dictionary<string, string> headers = routeSchema.Headers
                .ToDictionary(k => k.Key, v => v.Value);

            foreach (ApiRouteParameterSchema? headerParameter in routeSchema.GetHeaderSchemas())
            {
                headers.Add(headerParameter.Name, parameters[headerParameter.ParameterIndex].ToString());
            }

            return headers;
        }
    }
}
