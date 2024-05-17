using System;

namespace SereneApi.Resource.Schema.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public sealed class HttpHeaderAttribute : Attribute
    {
        public string Key { get; }

        public string Value { get; set; }

        public HttpHeaderAttribute(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Cannot be Null Or Empty", nameof(key));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Cannot be Null Or Empty", nameof(value));
            }

            Key = key;
            Value = value;
        }
    }
}
