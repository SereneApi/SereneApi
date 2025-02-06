using SereneApi.Resource.Exceptions;
using SereneApi.Resource.Schema.Attributes;
using SereneApi.Resource.Schema.Attributes.Request;
using SereneApi.Resource.Schema.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SereneApi.Resource.Schema
{
    [DebuggerDisplay("[{Method}] - {Template}")]
    internal sealed class ApiRouteSchema
    {
        public ILookup<ApiRouteParameterType, ApiRouteParameterSchema> Parameters { get; }

        public ApiResourceSchema ParentResource { get; }

        public HttpMethod Method { get; }

        public MethodInfo InvokedMethod { get; }

        public string? Template { get; }

        public string? Version { get; }

        public ApiRouteResponseSchema? Response { get; }

        public IReadOnlyCollection<ApiRouteHeaderSchema> Headers { get; }

        public ApiRouteSchema(ApiResourceSchema parentResource, MethodInfo method, HttpVersionAttribute? resourceVersionAttribute, IReadOnlyCollection<HttpHeaderAttribute> resourceHeaders)
        {
            HttpRequestAttribute request = method.GetCustomAttribute<HttpRequestAttribute>();

            if (request == null)
            {
                throw new ArgumentException($"Methods that do not implement the {nameof(HttpRequestAttribute)} are not supported.");
            }

            var routeVersion = method.GetCustomAttribute<HttpVersionAttribute>();

            if (routeVersion != null)
            {
                resourceVersionAttribute = routeVersion;
            }

            ParentResource = parentResource;
            Method = request.Method;
            Version = resourceVersionAttribute?.Version;
            InvokedMethod = method;
            Headers = ExtractHeaders(method, resourceHeaders);
            Response = ApiRouteResponseSchema.Create(method);

            Template = CompileRouteTemplate(request.RouteTemplate, out IReadOnlyDictionary<string, int> parameterTemplateIndexes);

            Parameters = BuildRouteParameters(method.GetParameters(), parameterTemplateIndexes).ToLookup(p => p.Type);

            ValidateParameters(method.Name);
            ValidateEndpointTemplateParameters(parameterTemplateIndexes, method.Name);
        }

        private static IReadOnlyCollection<ApiRouteHeaderSchema> ExtractHeaders(MethodInfo method, IReadOnlyCollection<HttpHeaderAttribute> resourceHeaders) =>
            method.GetCustomAttributes<HttpHeaderAttribute>()
                .Concat(resourceHeaders)
                .Select(r => new ApiRouteHeaderSchema(r.Key, r.Value))
                .ToList();

        private static string? CompileRouteTemplate(string? routeTemplate, out IReadOnlyDictionary<string, int> parameterTemplateIndexes)
        {
            if (string.IsNullOrWhiteSpace(routeTemplate))
            {
                parameterTemplateIndexes = new Dictionary<string, int>();

                return null;
            }

            MatchCollection matches = FindParameters(routeTemplate);

            Dictionary<string, int> parameterTemplateIndexesBuilder = new Dictionary<string, int>();

            for (int i = 0; i < matches.Count; i++)
            {
                string paramName = matches[i].Groups[1].Value;

                routeTemplate = routeTemplate.Replace($"{{{paramName}}}", $"{{{i}}}");

                if (!parameterTemplateIndexesBuilder.TryAdd(paramName, i))
                {
                    throw new ArgumentException($"Duplicate parameters found in Template, parameter name {paramName}", nameof(routeTemplate));
                }
            }

            parameterTemplateIndexes = parameterTemplateIndexesBuilder;

            return routeTemplate;
        }

        private void ValidateEndpointTemplateParameters(IReadOnlyDictionary<string, int> parameterTemplateMap, string methodName)
        {
            ApiRouteParameterSchema[] templateParameters = Parameters[ApiRouteParameterType.TemplateParameter].ToArray();

            if (parameterTemplateMap.Count != templateParameters.Length)
            {
                throw InvalidResourceSchemaException.TemplateParameterMissMatch(templateParameters, parameterTemplateMap, methodName);
            }

            if (templateParameters.Any(p => p.TemplateIndex == null))
            {
                throw new InvalidOperationException();
            }
        }

        private void ValidateParameters(string methodName)
        {
            ApiRouteParameterSchema[] contentParameters = Parameters[ApiRouteParameterType.Content].ToArray();

            if (contentParameters.Length > 1)
            {
                throw InvalidResourceSchemaException.MultipleContentSchemasFound(contentParameters, methodName);
            }
        }

        private static List<ApiRouteParameterSchema> BuildRouteParameters(ParameterInfo[] methodParameters, IReadOnlyDictionary<string, int> parameterTemplateMap)
        {
            List<ApiRouteParameterSchema> parameters = new List<ApiRouteParameterSchema>();

            for (int i = 0; i < methodParameters.Length; i++)
            {
                parameters.Add(ApiRouteParameterSchema.Create(i, methodParameters[i], parameterTemplateMap));
            }

            return parameters;
        }

        private static MatchCollection FindParameters(string routeTemplate)
        {
            // Matches anything inside curly braces
            Regex matchCurlyBraces = new Regex("{([^}]*)}");

            return matchCurlyBraces.Matches(routeTemplate);
        }
    }
}
