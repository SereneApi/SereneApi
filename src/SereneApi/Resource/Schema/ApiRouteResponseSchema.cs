using SereneApi.Resource.Exceptions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SereneApi.Resource.Schema
{
    internal sealed class ApiRouteResponseSchema
    {
        public Type? ResponseType { get; private set; }

        public static ApiRouteResponseSchema? Create(MethodInfo method)
        {
            ApiRouteResponseSchema schema = new ApiRouteResponseSchema();

            if (method.ReturnType == typeof(void))
            {
                return null;
            }

            if (method.ReturnType != typeof(Task) && (!method.ReturnType.IsGenericType || method.ReturnType.GetGenericTypeDefinition() != typeof(Task<>)))
            {
                throw InvalidResourceSchemaException.MethodMustBeAsync(method);
            }

            if (!method.ReturnType.IsGenericType)
            {
                return schema;
            }

            schema.ResponseType = method.ReturnType.GetGenericArguments().Single();

            return schema;
        }
    }
}
