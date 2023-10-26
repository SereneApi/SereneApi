using Castle.DynamicProxy;
using SereneApi.Helpers;
using SereneApi.Request.Attributes;
using SereneApi.Resource.Handling;
using SereneApi.Resource.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Resource
{
    public sealed class ApiResourceRegistry
    {
        private readonly IReadOnlyDictionary<Type, ApiResourceSchema> _resourceSchemas;

        private ApiResourceRegistry(IReadOnlyDictionary<Type, ApiResourceSchema> resourceSchemas)
        {
            _resourceSchemas = resourceSchemas;
        }

        public static ApiResourceRegistry Create()
        {
            Dictionary<Type, ApiResourceSchema> resourceSchemas = DiscoveryHelper
                .GetInterfacesImplementingAttribute<HttpResourceAttribute>()
                .ToDictionary(apiResource => apiResource, ApiResourceSchema.Create);

            return new ApiResourceRegistry(resourceSchemas);
        }

        public T CreateResourceHandler<T>() where T : class
        {
            ProxyGenerator generator = new ProxyGenerator();

            return generator.CreateInterfaceProxyWithoutTarget<T>(new ResourceHandler(_resourceSchemas[typeof(T)]));
        }
    }
}
