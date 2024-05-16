using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    public sealed class HttpPutRequestAttribute : HttpRequestAttribute
    {
        public HttpPutRequestAttribute() : base(HttpMethod.Put)
        {
        }

        public HttpPutRequestAttribute(string routeTemplate) : base(HttpMethod.Put, routeTemplate)
        {
        }
    }
}
