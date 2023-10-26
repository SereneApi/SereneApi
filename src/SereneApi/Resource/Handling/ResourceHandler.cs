using Castle.DynamicProxy;
using SereneApi.Helpers;
using SereneApi.Resource.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using SereneApi.Request.Factory;

namespace SereneApi.Resource.Handling
{
    internal sealed class ResourceHandler : IInterceptor
    {
        private readonly ApiResourceSchema _resourceSchema;



        public ResourceHandler(ApiResourceSchema resourceSchema)
        {
            _resourceSchema = resourceSchema;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!_resourceSchema.EndpointSchemas.TryGetValue(invocation.Method, out ApiEndpointSchema invocationSchema))
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
