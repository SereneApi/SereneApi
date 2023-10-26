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

        public IReadOnlyDictionary<MethodInfo, ApiEndpointSchema> EndpointSchemas { get; private set; } = null!;

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

            Dictionary<MethodInfo, ApiEndpointSchema> endpointSchemas = new Dictionary<MethodInfo, ApiEndpointSchema>();

            foreach (MethodInfo method in apiResourceType.GetMethods())
            {
                endpointSchemas.Add(method, ApiEndpointSchema.Create(method));
            }

            schema.EndpointSchemas = endpointSchemas;

            return schema;
        }
    }
}
