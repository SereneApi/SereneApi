using SereneApi.Resource.Exceptions;
using SereneApi.Resource.Schema.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace SereneApi.Resource.Schema
{
    [DebuggerDisplay("Resource: {Name} Routes: {RouteSchemas.Count}")]
    internal sealed class ApiResourceSchema
    {
        public string Name { get; private set; } = null!;

        public Type ResourceType { get; private set; } = null!;

        public IReadOnlyDictionary<MethodInfo, ApiRouteSchema> RouteSchemas { get; private set; } = null!;

        public static ApiResourceSchema Create(Type apiResourceType)
        {
            var schema = new ApiResourceSchema
            {
                ResourceType = apiResourceType,
                Name = GenerateName(apiResourceType)
            };

            schema.RouteSchemas = GenerateRoutes(apiResourceType, schema).ToDictionary(r => r.InvokedMethod);

            schema.ValidateRoutes();

            return schema;
        }

        private static string GenerateName(Type apiResourceType)
        {
            var resourceAttribute = apiResourceType.GetCustomAttribute<HttpResourceAttribute>()!;

            if (resourceAttribute.Resource != null)
            {
                return resourceAttribute.Resource;
            }

            return apiResourceType.Name.Substring(1, apiResourceType.Name.Length - 4);
        }

        private static IEnumerable<ApiRouteSchema> GenerateRoutes(Type apiResourceType, ApiResourceSchema schema)
        {
            var resourceVersionAttribute = apiResourceType.GetCustomAttribute<HttpVersionAttribute>();

            var httpHeaders = apiResourceType
                .GetCustomAttributes<HttpHeaderAttribute>()
                .ToList();

            foreach (var method in apiResourceType.GetMethods())
            {
                yield return new ApiRouteSchema(schema, method, resourceVersionAttribute, httpHeaders);
            }
        }

        private void ValidateRoutes()
        {
            List<IGrouping<HttpMethod, ApiRouteSchema>> methodGrouping = RouteSchemas.Values.GroupBy(v => v.Method).ToList();

            foreach (IGrouping<HttpMethod, ApiRouteSchema> methodGroup in methodGrouping)
            {
                var templateGrouping = methodGroup.GroupBy(v => v.Template);

                foreach (var routeSchema in templateGrouping)
                {
                    if (routeSchema.Count() > 1)
                    {
                        throw InvalidResourceSchemaException.DuplicateResourceTemplatesFound(routeSchema.ToArray(), ResourceType);
                    }
                }
            }
        }
    }
}
