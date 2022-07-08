using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the HttpMethod will respond to Delete request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockDeleteAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current httpMethod will response to Delete requests.
        /// </summary>
        public MockDeleteAttribute() : base("DELETE")
        {
        }

        /// <summary>
        /// Specifies the the current httpMethod will response to Delete requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the httpMethod parameters.</param>
        public MockDeleteAttribute(string endpointTemplate) : base("DELETE", endpointTemplate)
        {
        }
    }
}
