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
        private readonly ILookup<ApiRouteParameterType, ApiRouteParameterSchema> _routeParameterLookup;

        public ApiResourceSchema ParentResource { get;  }

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

            Template = CompileTemplate(request.RouteTemplate, out IReadOnlyDictionary<string, int> parameterTemplateMap);

            _routeParameterLookup = BuildRouteParameters(method.GetParameters(), parameterTemplateMap).ToLookup(p => p.Type);

            ValidateParameters(method.Name);
            ValidateEndpointTemplateParameters(parameterTemplateMap, method.Name);
        }
        
        public IEnumerable<ApiRouteParameterSchema> GetRouteParameterSchemas()
            => _routeParameterLookup[ApiRouteParameterType.TemplateParameter];

        public IEnumerable<ApiRouteParameterSchema> GetQuerySchemas()
            => _routeParameterLookup[ApiRouteParameterType.Query];

        public IEnumerable<ApiRouteParameterSchema> GetHeaderSchemas()
            => _routeParameterLookup[ApiRouteParameterType.Header];

        public ApiRouteParameterSchema? GetContentSchema()
            => _routeParameterLookup[ApiRouteParameterType.Content].SingleOrDefault();

        private static IReadOnlyCollection<ApiRouteHeaderSchema> ExtractHeaders(MethodInfo method, IReadOnlyCollection<HttpHeaderAttribute> resourceHeaders) =>
            method.GetCustomAttributes<HttpHeaderAttribute>()
                .Concat(resourceHeaders)
                .Select(r => new ApiRouteHeaderSchema(r.Key, r.Value))
                .ToList();

        private string? CompileTemplate(string? routeTemplate, out IReadOnlyDictionary<string, int> templateMap)
        {
            if (string.IsNullOrWhiteSpace(routeTemplate))
            {
                templateMap = new Dictionary<string, int>();

                return null;
            }

            MatchCollection matches = FindParameters(routeTemplate);

            Dictionary<string, int> parameterTemplateMap = new Dictionary<string, int>();

            for (int i = 0; i < matches.Count; i++)
            {
                string paramName = matches[i].Groups[1].Value;

                routeTemplate = routeTemplate.Replace($"{{{paramName}}}", $"{{{i}}}");

                if (!parameterTemplateMap.TryAdd(paramName, i))
                {
                    throw new ArgumentException($"Duplicate parameters found in Template, parameter name {paramName}", nameof(routeTemplate));
                }
            }

            templateMap = parameterTemplateMap;

            return routeTemplate;
        }

        private void ValidateEndpointTemplateParameters(IReadOnlyDictionary<string, int> parameterTemplateMap, string methodName)
        {
            ApiRouteParameterSchema[] parameters = GetRouteParameterSchemas().ToArray();

            if (parameterTemplateMap.Count != parameters.Length)
            {
                throw InvalidResourceSchemaException.TemplateParameterMissMatch(parameters, parameterTemplateMap, methodName);
            }

            if (parameters.Any(p => p.TemplateIndex == null))
            {
                throw new InvalidOperationException();
            }
        }

        private void ValidateParameters(string methodName)
        {
            ApiRouteParameterSchema[] contentParameters = _routeParameterLookup[ApiRouteParameterType.Content].ToArray();

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
            Regex matchCurlyBraces = new Regex("{([^}]*)}"); // Matches anything inside curly braces

            return matchCurlyBraces.Matches(routeTemplate);
        }
    }
}
