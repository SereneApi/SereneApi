using SereneApi.Resource.Schema.Attributes.Parameter;
using SereneApi.Resource.Schema.Enums;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SereneApi.Resource.Schema
{
    [DebuggerDisplay("[{Type}] - {Name} Index: {ParameterIndex} => {TemplateIndex}")]
    internal readonly struct ApiRouteParameterSchema
    {
        public string Name { get; }

        public int ParameterIndex { get; }

        public ApiRouteParameterType Type { get; }

        public int? TemplateIndex { get; }

        public ApiRouteParameterSchema(string name, int parameterIndex, ApiRouteParameterType type, int? templateIndex = null)
        {
            Name = name;
            ParameterIndex = parameterIndex;
            Type = type;
            TemplateIndex = templateIndex;
        }

        public static ApiRouteParameterSchema Create(int parameterIndex, ParameterInfo parameter, IReadOnlyDictionary<string, int> parameterTemplateMap)
        {
            HttpParameterAttribute? parameterAttribute = parameter.GetCustomAttribute<HttpParameterAttribute>();

            if (parameterAttribute != null)
            {
                return new ApiRouteParameterSchema(parameterAttribute.Name ?? parameter.Name, parameterIndex, parameterAttribute.Type);
            }

            if (parameterTemplateMap.TryGetValue(parameter.Name, out int index))
            {
                return new ApiRouteParameterSchema(parameter.Name, parameterIndex, ApiRouteParameterType.TemplateParameter, index);
            }

            return new ApiRouteParameterSchema(parameter.Name, parameterIndex, ApiRouteParameterType.TemplateParameter);
        }
    }
}
