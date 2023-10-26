using System.Net.Http;

namespace SereneApi.Request
{
    internal class ApiRequest
    {
        public HttpMethod Method { get; set; } = null!;

        public string Resource { get; set; } = null!;

        public string? Endpoint { get; set; }
    }
}
