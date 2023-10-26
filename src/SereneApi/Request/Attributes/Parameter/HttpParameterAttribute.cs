using SereneApi.Resource.Schema.Enums;
using System;

namespace SereneApi.Request.Attributes.Parameter
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class HttpParameterAttribute : Attribute
    {
        public string? Name { get; }

        public ApiEndpointParameterType Type { get; }

        protected HttpParameterAttribute(ApiEndpointParameterType type)
        {
            Name = null;
            Type = type;
        }

        protected HttpParameterAttribute(ApiEndpointParameterType type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be Null or Empty.", nameof(name));
            }

            Name = name;
            Type = type;
        }
    }
}
