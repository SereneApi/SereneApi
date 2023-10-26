using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    public sealed class HttpDeleteAttribute : HttpRequestAttribute
    {
        public HttpDeleteAttribute() : base(HttpMethod.Delete)
        {
        }

        public HttpDeleteAttribute(string endPointTemplate) : base(HttpMethod.Delete, endPointTemplate)
        {
        }
    }
}
