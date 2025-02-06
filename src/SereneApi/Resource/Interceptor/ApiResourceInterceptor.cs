using Castle.DynamicProxy;
using SereneApi.Request;
using SereneApi.Request.Factory;
using SereneApi.Request.Handler;
using SereneApi.Resource.Exceptions;
using SereneApi.Resource.Schema;

namespace SereneApi.Resource.Interceptor
{
    internal sealed class ApiResourceInterceptor : IInterceptor
    {
        private readonly ApiResourceSchema _resourceSchema;

        private readonly IApiRequestHandler _requestHandler;

        private readonly ApiRequestFactory _requestFactory;

        public ApiResourceInterceptor(ApiResourceSchema resourceSchema, ApiRequestFactory requestFactory, IApiRequestHandler requestHandler)
        {
            _resourceSchema = resourceSchema;
            _requestFactory = requestFactory;
            _requestHandler = requestHandler;
        }

        public async void Intercept(IInvocation resourceInvocation)
        {
            if (!_resourceSchema.RouteSchemas.TryGetValue(resourceInvocation.Method, out ApiRouteSchema routeSchema))
            {
                throw SchemaNotFoundException.RouteSchemaNotFound(_resourceSchema, resourceInvocation.Method);
            }

            IApiRequest request = _requestFactory.Build(routeSchema, resourceInvocation.Arguments);

            var response = await _requestHandler.SendAsync(request, request.CancellationToken);

            resourceInvocation.ReturnValue = response;
        }
    }
}
