using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Response.Content
{
    /// <summary>
    /// The default object used for HTTP failure messages.
    /// </summary>
    public class DefaultFailureResponse
    {
        /// <summary>
        /// Specifies the messages received.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultFailureResponse"/>.
        /// </summary>
        public DefaultFailureResponse()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultFailureResponse"/>.
        /// </summary>
        /// <param name="message">The failure message.</param>
        public DefaultFailureResponse([NotNull] string message)
        {
            if(string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
        }
    }
}
