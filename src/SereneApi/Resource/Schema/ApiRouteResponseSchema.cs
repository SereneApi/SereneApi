using SereneApi.Resource.Exceptions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SereneApi.Resource.Schema
{
    internal sealed class ApiRouteResponseSchema
    {
        public Type ResponseType { get; }

        private ApiRouteResponseSchema(Type responseType)
        {
            ResponseType = responseType;
        }

        public static ApiRouteResponseSchema? Create(MethodInfo method)
        {
            if (method.ReturnType == typeof(void))
            {
                return null;
            }

            if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return new ApiRouteResponseSchema(method.ReturnType.GetGenericArguments().Single());
            }

            if (method.ReturnType != typeof(Task))
            {
                throw InvalidResourceSchemaException.MethodMustBeAsync(method);
            }

            return null;
        }
    }
}
