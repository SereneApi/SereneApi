using SereneApi.Resource.Schema.Enums;
using System;

namespace SereneApi.Request.Attributes.Parameter
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class HttpParameterAttribute : Attribute
    {
        public string? Name { get; }

        public ApiRouteParameterType Type { get; }

        protected HttpParameterAttribute(ApiRouteParameterType type)
        {
            Name = null;
            Type = type;
        }

        protected HttpParameterAttribute(ApiRouteParameterType type, string name)
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
