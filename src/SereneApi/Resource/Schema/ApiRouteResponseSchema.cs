using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SereneApi.Resource.Schema
{
    internal sealed class ApiRouteResponseSchema
    {
        public bool IsAsync { get; private set; }

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
                schema.ResponseType = method.ReturnType;
                schema.IsAsync = false;

                return schema;
            }

            schema.IsAsync = true;

            if (!method.ReturnType.IsGenericType)
            {
                return schema;
            }

            schema.ResponseType = method.ReturnType.GetGenericArguments().Single();

            return schema;
        }
    }
}
