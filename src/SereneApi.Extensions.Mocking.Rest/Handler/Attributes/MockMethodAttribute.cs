using System;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the HttpMethod will respond to specified status of the request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MockMethodAttribute : Attribute
    {
        /// <summary>
        /// Specifies the httpMethod to be responded to.
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Specified the endpoint to be responded to.
        /// </summary>
        public string EndpointTemplate { get; }

        /// <summary>
        /// Specifies the the current httpMethod will response to Get requests.
        /// </summary>
        /// <param name="method">Sets the httpMethod to be responded to.</param>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the httpMethod parameters.</param>
        protected MockMethodAttribute(string method, string endpointTemplate = null)
        {
            if (endpointTemplate == null)
            {
                EndpointTemplate = string.Empty;
            }
            else
            {
                if (endpointTemplate.StartsWith('/'))
                {
                    throw new ArgumentException("A template cannot start with '/'", nameof(endpointTemplate));
                }

                EndpointTemplate = endpointTemplate;
            }

            HttpMethod = new HttpMethod(method);
        }
    }
}
