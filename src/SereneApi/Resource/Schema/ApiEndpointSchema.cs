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
    internal sealed class ApiEndpointSchema
    {
        public HttpMethod Method { get; private set; } = null!;

        public string? Template { get; private set; }

        public bool HasParameters { get; private set; }

        public bool HasContent { get; private set; }

        public bool HasQuery { get; private set; }

        public IReadOnlyCollection<ApiEndpointParameterSchema> Parameters { get; private set; } = null!;

        private ApiEndpointSchema()
        {
        }

        public static ApiEndpointSchema Create(MethodInfo method)
        {
            HttpRequestAttribute request = method.GetCustomAttribute<HttpRequestAttribute>();

            if (request == null)
            {
                throw new ArgumentException($"Methods that do not implement the {nameof(HttpRequestAttribute)} are not supported.");
            }

            ApiEndpointSchema schema = new ApiEndpointSchema
            {
                Method = request.Method,
                Template = request.EndpointTemplate,
            };

            IReadOnlyDictionary<string, int> parameterTemplateMap = schema.BuildParameterTemplateMap();

            schema.Parameters = BuildEndpointParameters(method.GetParameters(), parameterTemplateMap);
            schema.ValidateEndpointTemplateParameters(parameterTemplateMap, method.Name);
            schema.HasQuery = schema.Parameters.Any(p => p.Type == ApiEndpointParameterType.Query);
            schema.HasContent = schema.Parameters.Any(p => p.Type == ApiEndpointParameterType.Content);
            schema.HasParameters = schema.Parameters.Any(p => p.Type == ApiEndpointParameterType.TemplateParameter);

            return schema;
        }

        public IEnumerable<ApiEndpointParameterSchema> GetTemplateParameterSchemas() =>
            Parameters.Where(p => p.Type == ApiEndpointParameterType.TemplateParameter);

        public IEnumerable<ApiEndpointParameterSchema> GetQuerySchemas() =>
            Parameters.Where(p => p.Type == ApiEndpointParameterType.Query);

        public ApiEndpointParameterSchema? GetContactSchema() =>
            Parameters.SingleOrDefault(p => p.Type == ApiEndpointParameterType.Content);

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
            ApiEndpointParameterSchema[] parameters = GetTemplateParameterSchemas().ToArray();

            if (parameterTemplateMap.Count != parameters.Length)
            {
                throw InvalidResourceSchemaException.TemplateParameterMissMatch(parameters, parameterTemplateMap, methodName);
            }

            if (parameters.Any(p => p.TemplateIndex == null))
            {
                throw new InvalidOperationException();
            }
        }

        private static List<ApiEndpointParameterSchema> BuildEndpointParameters(ParameterInfo[] methodParameters, IReadOnlyDictionary<string, int> parameterTemplateMap)
        {
            List<ApiEndpointParameterSchema> parameters = new List<ApiEndpointParameterSchema>();

            for (int i = 0; i < methodParameters.Length; i++)
            {
                parameters.Add(ApiEndpointParameterSchema.Create(i, methodParameters[i], parameterTemplateMap));
            }

            return parameters;
        }

        private static MatchCollection FindParameters(string endpointTemplate)
        {
            Regex matchCurlyBraces = new Regex("{([^}]*)}"); // Matches anything inside curly braces

            return matchCurlyBraces.Matches(endpointTemplate);
        }
    }
}
