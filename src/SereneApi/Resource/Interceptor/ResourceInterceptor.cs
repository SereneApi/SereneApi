using Castle.DynamicProxy;
using SereneApi.Request.Factory;
using SereneApi.Resource.Schema;
using System;
using SereneApi.Resource.Settings;

namespace SereneApi.Resource.Interceptor
{
    internal sealed class ResourceInterceptor : IInterceptor
    {
        private readonly ApiResourceSchema _resourceSchema;

        private readonly IResourceSettings _resourceSettings;

        public ResourceInterceptor(ApiResourceSchema resourceSchema, IResourceSettings resourceSettings)
        {
            _resourceSchema = resourceSchema;
            _resourceSettings = resourceSettings;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!_resourceSchema.RouteSchemas.TryGetValue(invocation.Method, out ApiRouteSchema invocationSchema))
            {
                throw new InvalidOperationException();
            }

            RequestFactory requestFactory = new RequestFactory();

            requestFactory.AddEndpoint(invocation, invocationSchema);

            if (invocationSchema.HasQuery)
            {
                requestFactory.AddQuery(invocation, invocationSchema);
            }
        }
    }
}
