using System;
using System.Net.Http;

namespace SereneApi.Request.Attributes.Request
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpRequestAttribute : Attribute
    {
        public HttpMethod Method { get; }

        public string? RouteTemplate { get; }

        public HttpRequestAttribute(HttpMethod method)
        {
            Method = method;
            RouteTemplate = null;
        }

        public HttpRequestAttribute(HttpMethod method, string routeTemplate)
        {
            Method = method;
            RouteTemplate = routeTemplate;
        }
    }
}
