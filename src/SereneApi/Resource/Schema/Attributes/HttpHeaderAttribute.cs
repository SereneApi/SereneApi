using System;

namespace SereneApi.Resource.Schema.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public sealed class HttpHeaderAttribute : Attribute
    {
        public HttpHeaderAttribute(string key, string value)
        {
        }
    }    
}
