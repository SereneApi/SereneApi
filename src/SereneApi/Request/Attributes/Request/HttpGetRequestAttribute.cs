using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    public sealed class HttpGetRequestAttribute : HttpRequestAttribute
    {
        public HttpGetRequestAttribute() : base(HttpMethod.Get)
        {
        }

        public HttpGetRequestAttribute(string endPointTemplate) : base(HttpMethod.Get, endPointTemplate)
        {
        }
    }
}
