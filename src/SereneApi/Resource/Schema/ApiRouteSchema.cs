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
        public ApiResourceSchema ParentResource { get; private set; }
        
        public HttpMethod Method { get; private set; } = null!;

        public MethodInfo InvokedMethod { get; private set; } = null!;

        public string? Template { get; private set; }

        public string? Version { get; set; }

        public ApiRouteResponseSchema? Response { get; private set; }

        public IReadOnlyCollection<ApiRouteParameterSchema> Parameters { get; private set; } = null!;

        public IReadOnlyCollection<ApiRouteHeaderSchema> Headers { get; private set; } = null!;

        private ApiRouteSchema()
        {
        }

        public static ApiRouteSchema Create(ApiResourceSchema parentResource, MethodInfo method, HttpVersionAttribute? resourceVersionAttribute, IReadOnlyCollection<HttpHeaderAttribute> resourceHeaders)
        {
            HttpRequestAttribute request = method.GetCustomAttribute<HttpRequestAttribute>();

            List<HttpHeaderAttribute> routeHeaders = method.GetCustomAttributes<HttpHeaderAttribute>().ToList();

            routeHeaders.AddRange(resourceHeaders);

            if (request == null)
            {
                throw new ArgumentException($"Methods that do not implement the {nameof(HttpRequestAttribute)} are not supported.");
            }

            var routeVersion = method.GetCustomAttribute<HttpVersionAttribute>();

            if (routeVersion != null)
            {
                resourceVersionAttribute = routeVersion;
            }

            ApiRouteSchema schema = new ApiRouteSchema
            {
                ParentResource = parentResource,
                Method = request.Method,
                Template = request.RouteTemplate,
                Version = resourceVersionAttribute?.Version,
                InvokedMethod = method,
                Headers = routeHeaders.Select(r => new ApiRouteHeaderSchema(r.Key, r.Value)).ToList().AsReadOnly(),
                Response = ApiRouteResponseSchema.Create(method)
            };

            IReadOnlyDictionary<string, int> parameterTemplateMap = schema.BuildParameterTemplateMap();

            schema.Parameters = BuildRouteParameters(method.GetParameters(), parameterTemplateMap);
            schema.ValidateParameters(method.Name);
            schema.ValidateHeaders();
            schema.ValidateEndpointTemplateParameters(parameterTemplateMap, method.Name);

            return schema;
        }

        public IEnumerable<ApiRouteParameterSchema> GetRouteParameterSchemas()
            => Parameters.Where(p => p.Type == ApiRouteParameterType.TemplateParameter);

        public IEnumerable<ApiRouteParameterSchema> GetQuerySchemas()
            => Parameters.Where(p => p.Type == ApiRouteParameterType.Query);

        public IEnumerable<ApiRouteParameterSchema> GetHeaderSchemas()
            => Parameters.Where(p => p.Type == ApiRouteParameterType.Header);

        public ApiRouteParameterSchema? GetContentSchema()
            => Parameters.SingleOrDefault(p => p.Type == ApiRouteParameterType.Content);

        private IReadOnlyDictionary<string, int> BuildParameterTemplateMap()
        {
            if (string.IsNullOrWhiteSpace(Template))
            {
                return new Dictionary<string, int>();
            }

            MatchCollection matches = FindParameters(Template);

            Dictionary<string, int> parameterTemplateMap = new Dictionary<string, int>();

            for (int i = 0; i < matches.Count; i++)
            {
                string paramName = matches[i].Groups[1].Value;

                Template = Template.Replace($"{{{paramName}}}", $"{{{i}}}");

                if (!parameterTemplateMap.TryAdd(paramName, i))
                {
                    throw new ArgumentException($"Duplicate parameters found in Template, parameter name {paramName}", nameof(Template));
                }
            }

            return parameterTemplateMap;
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
            ApiRouteParameterSchema[] contentParameters = Parameters.Where(p => p.Type == ApiRouteParameterType.Content).ToArray();

            if (contentParameters.Length > 1)
            {
                throw InvalidResourceSchemaException.MultipleContentSchemasFound(contentParameters, methodName);
            }
        }

        private void ValidateHeaders()
        {
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

        private static MatchCollection FindParameters(string RouteTemplate)
        {
            Regex matchCurlyBraces = new Regex("{([^}]*)}"); // Matches anything inside curly braces

            return matchCurlyBraces.Matches(RouteTemplate);
        }
    }
}
