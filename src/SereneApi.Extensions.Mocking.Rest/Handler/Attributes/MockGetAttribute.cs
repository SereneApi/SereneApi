using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the HttpMethod will respond to Get request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockGetAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current httpMethod will response to Get requests.
        /// </summary>
        public MockGetAttribute() : base("GET")
        {
        }

        /// <summary>
        /// Specifies the the current httpMethod will response to Get requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the httpMethod parameters.</param>
        public MockGetAttribute(string endpointTemplate) : base("GET", endpointTemplate)
        {
        }
    }
}
