using SereneApi.Abstractions;
using System;

namespace SereneApi.Requests.Types
{
    internal class ApiVersion : IApiVersion
    {
        public string Version { get; }

        public ApiVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            Version = version;
        }
    }
}
