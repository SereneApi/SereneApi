using SereneApi.Request.Attributes.Request;
using SereneApi.Resource.Exceptions;
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
        public HttpMethod Method { get; private set; } = null!;

        public MethodInfo InvokedMethod { get; private set; } = null!;

        public string? Template { get; private set; }
        
        public bool HasParameters { get; private set; }

        public bool HasContent { get; private set; }

        public bool HasQuery { get; private set; }

        public ApiRouteResponseSchema? Response { get; private set; }

        public IReadOnlyCollection<ApiRouteParameterSchema> Parameters { get; private set; } = null!;

        private ApiRouteSchema()
        {
        }

        public static ApiRouteSchema Create(MethodInfo method)
        {
            HttpRequestAttribute request = method.GetCustomAttribute<HttpRequestAttribute>();

            if (request == null)
            {
                throw new ArgumentException($"Methods that do not implement the {nameof(HttpRequestAttribute)} are not supported.");
            }

            ApiRouteSchema schema = new ApiRouteSchema
            {
                Method = request.Method,
                Template = request.RouteTemplate,
                InvokedMethod = method,
                Response = ApiRouteResponseSchema.Create(method)
            };

            IReadOnlyDictionary<string, int> parameterTemplateMap = schema.BuildParameterTemplateMap();

            schema.Parameters = BuildRouteParameters(method.GetParameters(), parameterTemplateMap);
            schema.ValidateParameters(method.Name);
            schema.ValidateEndpointTemplateParameters(parameterTemplateMap, method.Name);
            schema.HasQuery = schema.Parameters.Any(p => p.Type == ApiRouteParameterType.Query);
            schema.HasContent = schema.Parameters.Any(p => p.Type == ApiRouteParameterType.Content);
            schema.HasParameters = schema.Parameters.Any(p => p.Type == ApiRouteParameterType.TemplateParameter);

            return schema;
        }
        
        public IEnumerable<ApiRouteParameterSchema> GetRouteParameterSchemas() =>
            Parameters.Where(p => p.Type == ApiRouteParameterType.TemplateParameter);

        public IEnumerable<ApiRouteParameterSchema> GetQuerySchemas() =>
            Parameters.Where(p => p.Type == ApiRouteParameterType.Query);

        public ApiRouteParameterSchema? GetContactSchema() =>
            Parameters.SingleOrDefault(p => p.Type == ApiRouteParameterType.Content);

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
