using SereneApi.Core.Requests;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Specifies that the Method will respond to specified status of the request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MockMethodAttribute : Attribute
    {
        /// <summary>
        /// Specifies the method to be responded to.
        /// </summary>
        public Method Method { get; }

        /// <summary>
        /// Specified the endpoint to be responded to.
        /// </summary>
        public string EndpointTemplate { get; }

        /// <summary>
        /// Specifies the the current method will response to Get requests.
        /// </summary>
        /// <param name="method">Sets the method to be responded to.</param>
        /// <param name="endpointTemplate">Sets the endpoint to be responded to, the endpoint can be bound to the method parameters.</param>
        protected MockMethodAttribute(Method method, string endpointTemplate = null)
        {
            if (endpointTemplate == null)
            {
                EndpointTemplate = string.Empty;
            }
            else
            {
                if (endpointTemplate.StartsWith('/'))
                {
                    throw new ArgumentException("A template cannot start with '/'", nameof(endpointTemplate));
                }

                EndpointTemplate = endpointTemplate;
            }

            Method = method;
        }
    }
}
