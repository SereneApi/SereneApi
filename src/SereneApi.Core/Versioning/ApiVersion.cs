using System;

namespace SereneApi.Core.Versioning
{
    public class ApiVersion : IApiVersion
    {
        private readonly string _version;

        public ApiVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            _version = version;
        }

        public string GetVersionString()
        {
            return _version;
        }
    }
}