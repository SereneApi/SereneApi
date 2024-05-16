using System;
using System.Net.Http;

namespace SereneApi.Resource.Schema.Attributes.Request
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HttpRequestAttribute : Attribute
    {
        public HttpMethod Method { get; }

        public string? RouteTemplate { get; }

        protected HttpRequestAttribute(HttpMethod method)
        {
            Method = method;
            RouteTemplate = null;
        }

        protected HttpRequestAttribute(HttpMethod method, string routeTemplate)
        {
            Method = method;
            RouteTemplate = routeTemplate;
        }
    }
}
