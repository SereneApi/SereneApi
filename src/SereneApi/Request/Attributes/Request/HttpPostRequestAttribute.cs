using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    public sealed class HttpPostRequestAttribute : HttpRequestAttribute
    {
        public HttpPostRequestAttribute() : base(HttpMethod.Post)
        {
        }

        public HttpPostRequestAttribute(string routeTemplate) : base(HttpMethod.Post, routeTemplate)
        {
        }
    }
}
