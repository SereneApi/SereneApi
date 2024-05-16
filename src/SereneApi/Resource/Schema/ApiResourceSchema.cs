using SereneApi.Request.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SereneApi.Resource.Schema
{
    [DebuggerDisplay("Resource: {Name}")]
    internal sealed class ApiResourceSchema
    {
        public string Name { get; private set; } = null!;

        public IReadOnlyDictionary<MethodInfo, ApiRouteSchema> RouteSchemas { get; private set; } = null!;

        private ApiResourceSchema()
        {
        }

        public static ApiResourceSchema Create(Type apiResourceType)
        {
            HttpResourceAttribute resourceAttribute = apiResourceType.GetCustomAttribute<HttpResourceAttribute>()!;

            ApiResourceSchema schema = new ApiResourceSchema();

            if (resourceAttribute.Resource != null)
            {
                schema.Name = resourceAttribute.Resource;
            }
            else
            {
                schema.Name = apiResourceType.Name.Substring(1, apiResourceType.Name.Length - 4);
            }

            Dictionary<MethodInfo, ApiRouteSchema> routeSchemas = new Dictionary<MethodInfo, ApiRouteSchema>();

            foreach (MethodInfo method in apiResourceType.GetMethods())
            {
                routeSchemas.Add(method, ApiRouteSchema.Create(method));
            }

            schema.RouteSchemas = routeSchemas;

            return schema;
        }
    }
}
