using System.Net.Http;

namespace SereneApi.Resource.Schema.Attributes.Request
{
    public sealed class HttpPatchRequestAttribute : HttpRequestAttribute
    {
        public HttpPatchRequestAttribute() : base(HttpMethod.Patch)
        {
        }

        public HttpPatchRequestAttribute(string routeTemplate) : base(HttpMethod.Patch, routeTemplate)
        {
        }
    }
}
