using Castle.DynamicProxy;
using SereneApi.Resource.Schema;
using SereneApi.Resource.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SereneApi.Tests")]
[assembly: InternalsVisibleTo("Testing")]
namespace SereneApi.Resource
{
    internal sealed class ApiResourceProvider
    {
        private readonly ProxyGenerator _resourceHandlerGenerator = new ProxyGenerator();

        public IReadOnlyDictionary<Type, ApiResourceSchema> ResourceSchemas { get; set; }

        public ApiResourceProvider(IApiResourceCollection apiResourceCollection)
        {
            ResourceSchemas = apiResourceCollection
                .GetApiResourceTypes()
                .ToDictionary(apiResource => apiResource, ApiResourceSchema.Create);
        }

        public T CreateResourceHandler<T>() where T : class
            => _resourceHandlerGenerator.CreateInterfaceProxyWithoutTarget<T>(new ApiResourceSynchronousInterceptor(ResourceSchemas[typeof(T)]));
    }
}
