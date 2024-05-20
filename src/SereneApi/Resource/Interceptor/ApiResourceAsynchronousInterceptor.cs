using Castle.DynamicProxy;
using SereneApi.Request;
using SereneApi.Request.Factory;
using SereneApi.Request.Handler;
using SereneApi.Resource.Exceptions;
using SereneApi.Resource.Schema;

namespace SereneApi.Resource.Interceptor
{
    internal sealed class ApiResourceAsynchronousInterceptor : IInterceptor
    {
        private readonly ApiResourceSchema _resourceSchema;

        private readonly IApiRequestHandler _requestHandler;

        public ApiResourceAsynchronousInterceptor(ApiResourceSchema resourceSchema)
        {
            _resourceSchema = resourceSchema;
        }

        public async void Intercept(IInvocation resourceInvocation)
        {
            if (!_resourceSchema.RouteSchemas.TryGetValue(resourceInvocation.Method, out ApiRouteSchema routeSchema))
            {
                throw SchemaNotFoundException.RouteSchemaNotFound(_resourceSchema, resourceInvocation.Method);
            }

            ApiRequestFactory apiRequestFactory = new ApiRequestFactory();

            IApiRequest request = apiRequestFactory.Build(routeSchema, resourceInvocation.Arguments);

            var response = await _requestHandler.ExecuteAsync(request);
        }
    }
}
