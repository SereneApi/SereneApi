using System.Net.Http;

namespace SereneApi.Resource.Schema.Attributes.Request
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
