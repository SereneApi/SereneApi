using SereneApi.Resource.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Resource.Exceptions
{
    public class InvalidResourceSchemaException : Exception
    {
        private InvalidResourceSchemaException(string message) : base(message)
        {
        }

        internal static InvalidResourceSchemaException TemplateParameterMissMatch(ApiEndpointParameterSchema[] parameters, IReadOnlyDictionary<string, int> parameterTemplateMap, string methodName)
        {
            IEnumerable<string> missingParameters;

            if (parameterTemplateMap.Count > parameters.Length)
            {
                missingParameters = parameterTemplateMap.Keys.Except(parameters.Select(p => p.Name));

                return new InvalidResourceSchemaException($"The Method {methodName} Specifies Parameters in its Template that do not map to the Method Parameters [{string.Join(',', missingParameters)}]");
            }

            missingParameters = parameters.Select(p => p.Name).Except(parameterTemplateMap.Keys);

            return new InvalidResourceSchemaException($"The Method {methodName} contains Method Parameters that do not map to the Endpoint Template [{string.Join(',', missingParameters)}]");
        }
    }
}
