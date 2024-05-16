using Castle.DynamicProxy;
using SereneApi.Helpers;
using SereneApi.Resource.Schema;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Request.Factory
{
    internal class RequestFactory
    {
        private string? _route;

        private string? _query;

        public void AddEndpoint(IInvocation invocation, ApiRouteSchema routeSchema)
        {
            if (!routeSchema.HasParameters)
            {
                _route = routeSchema.Template;
            }

            var routeParameters = routeSchema
                .GetRouteParameterSchemas()
                .OrderBy(p => p.TemplateIndex)
                .ToArray();

            object[] parameters = new object[routeParameters.Length];

            for (int i = 0; i < routeParameters.Length; i++)
            {
                parameters[i] = invocation.Arguments[routeParameters[i].ParameterIndex];
            }

            _route = string.Format(routeSchema.Template!, parameters);
        }

        public void AddQuery(IInvocation invocation, ApiRouteSchema routeSchema)
        {
            var queryParameters = routeSchema.GetQuerySchemas().ToArray();

            Dictionary<string, string> querySections = new Dictionary<string, string>();

            for (int i = 0; i < queryParameters.Length; i++)
            {
                querySections.Add(queryParameters[i].Name, invocation.Arguments[queryParameters[i].ParameterIndex].ToString());
            }

            _query = QueryHelper.BuildQueryString(querySections);
        }
    }
}
