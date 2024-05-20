using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Request
{
    public interface IApiRequest
    {
        public HttpMethod Method { get; }

        public string? Route { get; }

        public string? Version { get; }

        public string? Query { get; }
        
        public string FullRoute { get; }

        public object? Content { get; }

        public IReadOnlyDictionary<string, string> Headers { get; }
    }
}
