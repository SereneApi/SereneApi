using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    public sealed class HttpGetRequestAttribute : HttpRequestAttribute
    {
        public HttpGetRequestAttribute() : base(HttpMethod.Get)
        {
        }

        public HttpGetRequestAttribute(string routeTemplate) : base(HttpMethod.Get, routeTemplate)
        {
        }
    }
}
