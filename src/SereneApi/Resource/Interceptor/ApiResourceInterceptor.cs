using Castle.DynamicProxy;
using SereneApi.Request.Factory;
using SereneApi.Resource.Schema;
using System;
using SereneApi.Resource.Settings;

namespace SereneApi.Resource.Interceptor
{
    internal sealed class ApiResourceInterceptor : IInterceptor
    {
        private readonly ApiResourceSchema _resourceSchema;

        public ApiResourceInterceptor(ApiResourceSchema resourceSchema)
        {
            _resourceSchema = resourceSchema;
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
