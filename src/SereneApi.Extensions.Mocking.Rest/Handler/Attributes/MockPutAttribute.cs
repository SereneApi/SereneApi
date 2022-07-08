using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the Method will respond to Put request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockPutAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current method will response to Put requests.
        /// </summary>
        public MockPutAttribute() : base(Method.Put)
        {
        }

        /// <summary>
        /// Specifies the the current method will response to Put requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the method parameters.</param>
        public MockPutAttribute(string endpointTemplate) : base(Method.Put, endpointTemplate)
        {
        }
    }
}
