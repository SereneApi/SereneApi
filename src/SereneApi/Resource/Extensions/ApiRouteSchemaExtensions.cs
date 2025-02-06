using SereneApi.Helpers;
using SereneApi.Resource.Schema.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace SereneApi.Resource.Schema
{
    internal static class ApiRouteSchemaExtensions
    {
        public static T GetParameter<T>(this ApiRouteSchema routeSchema, IReadOnlyList<object> parameters, ApiRouteParameterType parameterType)
            => routeSchema.GetParameters(parameters, parameterType).Cast<T>().SingleOrDefault();

        public static object? GetParameter(this ApiRouteSchema routeSchema, IReadOnlyList<object> parameters, ApiRouteParameterType parameterType)
            => routeSchema.GetParameters(parameters, parameterType).SingleOrDefault();

        public static IEnumerable<object> GetParameters(this ApiRouteSchema routeSchema, IReadOnlyList<object> parameters, ApiRouteParameterType parameterType)
            => routeSchema.Parameters[parameterType].Select(parameterSchema => parameters[parameterSchema.ParameterIndex]);

        public static string? GetRoute(this ApiRouteSchema routeSchema, IReadOnlyList<object> parameters)
        {
            var routeParameters = routeSchema.Parameters[ApiRouteParameterType.TemplateParameter].ToArray();

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

        public static string GetQuery(this ApiRouteSchema routeSchema, IReadOnlyList<object> parameters)
        {
            Dictionary<string, string> querySections = routeSchema
                .Parameters[ApiRouteParameterType.Query]
                .ToDictionary(queryParameter => queryParameter.Name, queryParameter => parameters[queryParameter.ParameterIndex].ToString());

            return QueryHelper.BuildQueryString(querySections);
        }

        public static IReadOnlyDictionary<string, string> GetHeaders(this ApiRouteSchema routeSchema, IReadOnlyList<object> parameters)
        {
            var headers = routeSchema.Headers.ToDictionary(k => k.Key, v => v.Value);

            foreach (var headerParameter in routeSchema.Parameters[ApiRouteParameterType.Header])
            {
                headers.Add(headerParameter.Name, parameters[headerParameter.ParameterIndex].ToString());
            }

            return headers;
        }
    }
}
