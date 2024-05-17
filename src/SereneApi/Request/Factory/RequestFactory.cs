using Castle.DynamicProxy;
using SereneApi.Helpers;
using SereneApi.Resource.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace SereneApi.Request.Factory
{
    internal class RequestFactory
    {
        private string? _route;

        private string? _query;

        private string? _version;

        private object? _content;

        private HttpMethod? _method;

        public void SetRoute(IInvocation resourceInvocation, ApiRouteSchema routeSchema)
        {
            if (!routeSchema.HasParameters)
            {
                _route = routeSchema.Template;
            }

            ApiRouteParameterSchema[] routeParameters = routeSchema
                .GetRouteParameterSchemas()
                .OrderBy(p => p.TemplateIndex)
                .ToArray();

            object[] parameters = new object[routeParameters.Length];

            for (int i = 0; i < routeParameters.Length; i++)
            {
                parameters[i] = resourceInvocation.Arguments[routeParameters[i].ParameterIndex];
            }

            _route = string.Format(routeSchema.Template!, parameters);
        }

        public void SetQuery(IInvocation resourceInvocation, ApiRouteSchema routeSchema)
        {
            ApiRouteParameterSchema[] queryParameters = routeSchema.GetQuerySchemas().ToArray();

            Dictionary<string, string> querySections = new Dictionary<string, string>();

            foreach (ApiRouteParameterSchema? queryParameter in queryParameters)
            {
                querySections.Add(queryParameter.Name, resourceInvocation.Arguments[queryParameter.ParameterIndex].ToString());
            }

            _query = QueryHelper.BuildQueryString(querySections);
        }

        public void SetVersion(IInvocation resourceInvocation, ApiRouteSchema routeSchema)
            => _version = routeSchema.Version;

        public void SetContent(IInvocation resourceInvocation, ApiRouteSchema routeSchema)
        {
            ApiRouteParameterSchema contentSchema = routeSchema.GetContentSchema()!;

            _content = resourceInvocation.Arguments[contentSchema.ParameterIndex];
        }

        public void SetMethod(HttpMethod routeSchemaMethod)
            => _method = routeSchemaMethod;
    }
}
