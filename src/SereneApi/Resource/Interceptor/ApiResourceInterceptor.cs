using Castle.DynamicProxy;
using SereneApi.Request;
using SereneApi.Request.Factory;
using SereneApi.Resource.Exceptions;
using SereneApi.Resource.Schema;

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
                throw SchemaNotFoundException.RouteSchemaNotFound(_resourceSchema, resourceInvocation.Method);
            }

            RequestFactory requestFactory = new RequestFactory();

            ApiRequest request = requestFactory.Build(routeSchema, resourceInvocation.Arguments);
        }
    }
}
