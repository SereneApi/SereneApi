using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the HttpMethod will respond to Post request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockPostAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current httpMethod will response to Post requests.
        /// </summary>
        public MockPostAttribute() : base("POST")
        {
        }

        /// <summary>
        /// Specifies the the current httpMethod will response to Post requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the httpMethod parameters.</param>
        public MockPostAttribute(string endpointTemplate) : base("POST", endpointTemplate)
        {
        }
    }
}
