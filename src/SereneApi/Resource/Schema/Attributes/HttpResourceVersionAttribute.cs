using System;

namespace SereneApi.Resource.Schema.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public sealed class HttpResourceVersionAttribute : Attribute
    {
        public string Version { get; }

        public HttpResourceVersionAttribute(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Version cannot be null or empty");
            }

            Version = version;
        }
    }
}
