using SereneApi.Abstractions;
using System;

namespace SereneApi.Requests.Types
{
    internal class ApiVersion : IApiVersion
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
