using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the Method will respond to Delete request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MockDeleteAttribute : MockMethodAttribute
    {
        /// <summary>
        /// Specifies the the current method will response to Delete requests.
        /// </summary>
        public MockDeleteAttribute() : base(Method.Delete)
        {
        }

        /// <summary>
        /// Specifies the the current method will response to Delete requests.
        /// </summary>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the method parameters.</param>
        public MockDeleteAttribute(string endpointTemplate) : base(Method.Delete, endpointTemplate)
        {
        }
    }
}
