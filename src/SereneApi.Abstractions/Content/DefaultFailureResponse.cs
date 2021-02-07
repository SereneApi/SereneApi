using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Content
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
        /// Specifies further message details.
        /// </summary>
        public string MessageDetail { get; set; }

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
        /// <param name="messageDetail">Further details of the message.</param>
        public DefaultFailureResponse([NotNull] string message, string messageDetail = null)
        {
            if(string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
            MessageDetail = messageDetail;
        }
    }
}
