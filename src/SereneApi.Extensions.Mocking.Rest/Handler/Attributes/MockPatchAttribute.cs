using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the HttpMethod will respond to Patch request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockPatchAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current httpMethod will response to Patch requests.
        /// </summary>
        public MockPatchAttribute() : base("PATCH")
        {
        }

        /// <summary>
        /// Specifies the the current httpMethod will response to Patch requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the httpMethod parameters.</param>
        public MockPatchAttribute(string endpointTemplate) : base("PATCH", endpointTemplate)
        {
        }
    }
}
