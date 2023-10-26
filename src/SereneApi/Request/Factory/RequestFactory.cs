using Castle.DynamicProxy;
using SereneApi.Resource.Schema;
using System.Collections.Generic;
using System.Linq;
using SereneApi.Helpers;

namespace SereneApi.Request.Factory
{
    internal class RequestFactory
    {
        private string? _endpoint;

        private string? _query;

        public void AddEndpoint(IInvocation invocation, ApiEndpointSchema endpointSchema)
        {
            if (!endpointSchema.HasParameters)
            {
                _endpoint = endpointSchema.Template;
            }

            var templateParameters = endpointSchema
                .GetTemplateParameterSchemas()
                .OrderBy(p => p.TemplateIndex)
                .ToArray();

            object[] parameters = new object[templateParameters.Length];

            for (int i = 0; i < templateParameters.Length; i++)
            {
                parameters[i] = invocation.Arguments[templateParameters[i].ParameterIndex];
            }

            _endpoint = string.Format(endpointSchema.Template!, parameters);
        }

        public void AddQuery(IInvocation invocation, ApiEndpointSchema endpointSchema)
        {
            var queryParameters = endpointSchema.GetQuerySchemas().ToArray();

            Dictionary<string, string> querySections = new Dictionary<string, string>();

            for (int i = 0; i < queryParameters.Length; i++)
            {
                querySections.Add(queryParameters[i].Name, invocation.Arguments[queryParameters[i].ParameterIndex].ToString());
            }

            _endpoint = QueryHelper.BuildQueryString(querySections);
        }
    }
}
