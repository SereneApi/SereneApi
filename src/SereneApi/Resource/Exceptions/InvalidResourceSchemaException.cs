using SereneApi.Resource.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Resource.Exceptions
{
    public sealed class InvalidResourceSchemaException : Exception
    {
        private InvalidResourceSchemaException(string message) : base(message)
        {
        }

        internal static InvalidResourceSchemaException TemplateParameterMissMatch(ApiRouteParameterSchema[] parameters, IReadOnlyDictionary<string, int> parameterTemplateMap, string methodName)
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

        internal static InvalidResourceSchemaException MultipleContentSchemasFound(ApiRouteParameterSchema[] parameters, string methodName)
        {
            return new InvalidResourceSchemaException($"The Method {methodName} contains multiple content parameters [{string.Join(',', parameters.Select(p => p.Name))}], no more than 1 can be defined at a time.");
        }

        internal static InvalidResourceSchemaException DuplicateResourceTemplatesFound(ApiRouteSchema[] routes, Type resourceType)
        {
            return new InvalidResourceSchemaException($"The Resource [{resourceType.Name}] has Multiple Routes [{string.Join(',', routes.Select(r => r.InvokedMethod.Name))}] with the Matching Templates [{string.Join(',', routes.Select(r => r.Template))}]");
        }
    }
}
