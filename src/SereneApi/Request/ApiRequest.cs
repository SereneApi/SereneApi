using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Request
{
    internal sealed class ApiRequest : IApiRequest
    {
        public HttpMethod Method { get; }

        public string? Route { get; set; }

        public string? Version { get; set; }

        public string? Query { get; set; }

        public object? Content { get; set; }

        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        public ApiRequest(HttpMethod method)
        {
            Method = method;
        }
    }
}
