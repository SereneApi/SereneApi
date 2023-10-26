using SereneApi.Request.Attributes.Parameter;
using SereneApi.Resource.Schema.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SereneApi.Resource.Schema
{
    [DebuggerDisplay("[{Type}] - {Name} Index: {ParameterIndex} => {TemplateIndex}")]
    internal sealed class ApiEndpointParameterSchema
    {
        public string Name { get; private set; } = null!;

        public int ParameterIndex { get; private set; }

        public ApiEndpointParameterType Type { get; private set; }

        public int? TemplateIndex { get; private set; }

        public static ApiEndpointParameterSchema Create(int parameterIndex, ParameterInfo parameter, IReadOnlyDictionary<string, int> parameterTemplateMap)
        {
            ApiEndpointParameterSchema schema = new ApiEndpointParameterSchema
            {
                Name = parameter.Name,
                ParameterIndex = parameterIndex
            };

            HttpParameterAttribute? parameterAttribute = parameter.GetCustomAttribute<HttpParameterAttribute>();

            if (parameterAttribute != null)
            {
                if (!string.IsNullOrEmpty(parameterAttribute.Name))
                {
                    schema.Name = parameterAttribute.Name;
                }

                schema.Type = parameterAttribute.Type;
            }
            else
            {
                schema.Type = ApiEndpointParameterType.TemplateParameter;

                if (parameterTemplateMap.TryGetValue(parameter.Name, out int index))
                {
                    schema.TemplateIndex = index;
                }

            }

            return schema;
        }
    }
}
