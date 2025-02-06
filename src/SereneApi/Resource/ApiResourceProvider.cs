using Castle.DynamicProxy;
using SereneApi.Request.Factory;
using SereneApi.Request.Handler;
using SereneApi.Resource.Interceptor;
using SereneApi.Resource.Schema;
using SereneApi.Resource.Source;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SereneApi.Tests")]
[assembly: InternalsVisibleTo("Testing")]
namespace SereneApi.Resource
{
    internal sealed class ApiResourceProvider
    {
        private readonly ProxyGenerator _resourceHandlerGenerator = new ProxyGenerator();
        private readonly IReadOnlyDictionary<Type, ApiResourceSchema> _resourceSchemas;
        private readonly ApiRequestFactory _requestFactory;
        private readonly IApiRequestHandler _requestHandler;

        public ApiResourceProvider(IApiResourceCollection apiResourceCollection, ApiRequestFactory requestFactory, IApiRequestHandler requestHandler)
        {
            _requestFactory = requestFactory;
            _requestHandler = requestHandler;
            _resourceSchemas = apiResourceCollection.GetApiResourcesAsDictionary();
        }

        public T CreateResourceHandler<T>() where T : class
            => _resourceHandlerGenerator.CreateInterfaceProxyWithoutTarget<T>(new ApiResourceInterceptor(_resourceSchemas[typeof(T)], _requestFactory, _requestHandler));
    }
}
