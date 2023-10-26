using System;
using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpRequestAttribute : Attribute
    {
        public HttpMethod Method { get; }

        public string? EndpointTemplate { get; }

        public HttpRequestAttribute(HttpMethod method)
        {
            Method = method;
            EndpointTemplate = null;
        }

        public HttpRequestAttribute(HttpMethod method, string endpointTemplate)
        {
            Method = method;
            EndpointTemplate = endpointTemplate;
        }
    }
}
