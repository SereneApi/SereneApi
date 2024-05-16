using System;

namespace SereneApi.Resource.Schema.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class HttpResourceAttribute : Attribute
    {
        public string? Resource { get; }

        public HttpResourceAttribute()
        {
            Resource = null;
        }

        public HttpResourceAttribute(string resource)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentException("Cannot be Null Or Empty", nameof(resource));
            }

            Resource = resource;
        }
    }
}
