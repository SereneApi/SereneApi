using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Response.Content
{
    /// <summary>
    /// The default object used for HTTP failure messages.
    /// </summary>
    public class DefaultFailureMessage
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
        /// Creates a new instance of <see cref="DefaultFailureMessage"/>.
        /// </summary>
        public DefaultFailureMessage()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="DefaultFailureMessage"/>.
        /// </summary>
        /// <param name="message">The failure message.</param>
        /// <param name="messageDetail">Further details of the message.</param>
        public DefaultFailureMessage([NotNull] string message, string messageDetail = null)
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
