using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the Method will respond to Post request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockPostAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current method will response to Post requests.
        /// </summary>
        public MockPostAttribute() : base(Method.Post)
        {
        }

        /// <summary>
        /// Specifies the the current method will response to Post requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the method parameters.</param>
        public MockPostAttribute(string endpointTemplate) : base(Method.Post, endpointTemplate)
        {
        }
    }
}
