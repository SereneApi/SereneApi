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
            HttpResourceAttribute resourceAttribute = apiResourceType.GetCustomAttribute<HttpResourceAttribute>()!;
            HttpVersionAttribute? resourceVersionAttribute = apiResourceType.GetCustomAttribute<HttpVersionAttribute>()!;

            ApiResourceSchema schema = new ApiResourceSchema
            {
                ResourceType = apiResourceType
            };

            if (resourceAttribute.Resource != null)
            {
                schema.Name = resourceAttribute.Resource;
            }
            else
            {
                schema.Name = apiResourceType.Name.Substring(1, apiResourceType.Name.Length - 4);
            }

            IReadOnlyCollection<HttpHeaderAttribute> httpHeaders = apiResourceType
                .GetCustomAttributes<HttpHeaderAttribute>()
                .ToList()
                .AsReadOnly();

            Dictionary<MethodInfo, ApiRouteSchema> routeSchemas = new Dictionary<MethodInfo, ApiRouteSchema>();

            foreach (MethodInfo method in apiResourceType.GetMethods())
            {
                routeSchemas.Add(method, ApiRouteSchema.Create(schema, method, resourceVersionAttribute, httpHeaders));
            }

            schema.RouteSchemas = routeSchemas;

            schema.ValidateRoutes();

            return schema;
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
