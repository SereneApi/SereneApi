using System.Net.Http;

namespace SereneApi.Request
{
    public interface IApiRequest
    {
        public HttpMethod Method { get; }

        public string? Route { get; }

        public string? Version { get; }

        public string? Query { get; }

        public object? Content { get; }
    }
}
