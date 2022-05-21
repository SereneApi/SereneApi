using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the Method will respond to Get request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockGetAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current method will response to Get requests.
        /// </summary>
        public MockGetAttribute() : base(Method.Get)
        {
        }

        /// <summary>
        /// Specifies the the current method will response to Get requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the method parameters.</param>
        public MockGetAttribute(string endpointTemplate) : base(Method.Get, endpointTemplate)
        {
        }
    }
}
