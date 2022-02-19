using System;

namespace SereneApi.Core.Http.Responses.Types
{
    /// <summary>
    /// The default object used for HTTP failure messages.
    /// </summary>
    public class FailureResponse
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
        /// Creates a new instance of <see cref="FailureResponse"/>.
        /// </summary>
        public FailureResponse()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FailureResponse"/>.
        /// </summary>
        /// <param name="message">The failure message.</param>
        /// <param name="messageDetail">Further details of the message.</param>
        public FailureResponse(string message, string messageDetail = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
            MessageDetail = messageDetail;
        }
    }
}