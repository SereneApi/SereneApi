using Castle.DynamicProxy;
using SereneApi.Request.Factory;
using SereneApi.Resource.Schema;
using System;

namespace SereneApi.Resource.Interceptor
{
    internal sealed class ApiResourceInterceptor : IInterceptor
    {
        private readonly ApiResourceSchema _resourceSchema;

        public ApiResourceInterceptor(ApiResourceSchema resourceSchema)
        {
            _resourceSchema = resourceSchema;
        }

        public void Intercept(IInvocation resourceInvocation)
        {
            if (!_resourceSchema.RouteSchemas.TryGetValue(resourceInvocation.Method, out ApiRouteSchema routeSchema))
            {
                throw new InvalidOperationException();
            }

            RequestFactory requestFactory = new RequestFactory();

            requestFactory.SetVersion(resourceInvocation, routeSchema);
            requestFactory.SetRoute(resourceInvocation, routeSchema);
            requestFactory.SetMethod(routeSchema.Method);

            if (routeSchema.HasQuery)
            {
                requestFactory.SetQuery(resourceInvocation, routeSchema);
            }

            if (routeSchema.HasContent)
            {
                requestFactory.SetContent(resourceInvocation, routeSchema);
            }
        }
    }
}
