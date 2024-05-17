using SereneApi.Resource.Schema;
using System;
using System.Reflection;

namespace SereneApi.Resource.Exceptions
{
    internal sealed class SchemaNotFoundException : Exception
    {
        private SchemaNotFoundException(string message) : base(message)
        {
        }

        public static SchemaNotFoundException RouteSchemaNotFound(ApiResourceSchema resourceSchema, MethodInfo method)
            => new SchemaNotFoundException($"The Resource Schema [{resourceSchema.Name}] does not contain a Route Schema for Method [{method.Name}]");
    }
}
