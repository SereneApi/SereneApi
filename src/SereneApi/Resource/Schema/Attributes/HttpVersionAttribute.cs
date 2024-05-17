using System;

namespace SereneApi.Resource.Schema.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public sealed class HttpVersionAttribute : Attribute
    {
        public string Version { get; }

        public HttpVersionAttribute(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Version cannot be null or empty");
            }

            Version = version;
        }
    }
}
