using SereneApi.Resource.Schema.Enums;
using System;

namespace SereneApi.Resource.Schema.Attributes.Parameter
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class HttpHeaderParameterAttribute : HttpParameterAttribute
    {
        public HttpHeaderParameterAttribute(string key) : base(ApiRouteParameterType.Header, key)
        {
        }
    }
}
