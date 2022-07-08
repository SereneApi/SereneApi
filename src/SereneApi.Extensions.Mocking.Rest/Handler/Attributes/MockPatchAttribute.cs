using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the Method will respond to Patch request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockPatchAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current method will response to Patch requests.
        /// </summary>
        public MockPatchAttribute() : base(Method.Patch)
        {
        }

        /// <summary>
        /// Specifies the the current method will response to Patch requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the method parameters.</param>
        public MockPatchAttribute(string endpointTemplate) : base(Method.Patch, endpointTemplate)
        {
        }
    }
}
