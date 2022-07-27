using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the HttpMethod will respond to Put request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockPutAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current httpMethod will response to Put requests.
        /// </summary>
        public MockPutAttribute() : base("PUT")
        {
        }

        /// <summary>
        /// Specifies the the current httpMethod will response to Put requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the httpMethod parameters.</param>
        public MockPutAttribute(string endpointTemplate) : base("PUT", endpointTemplate)
        {
        }
    }
}
