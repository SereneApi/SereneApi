using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    internal class HttpPatchRequestAttribute : HttpRequestAttribute
    {
        public HttpPatchRequestAttribute() : base(HttpMethod.Patch)
        {
        }

        public HttpPatchRequestAttribute(string endPointTemplate) : base(HttpMethod.Patch, endPointTemplate)
        {
        }
    }
}
