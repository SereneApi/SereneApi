using System;

namespace SereneApi.Core.Http.Authentication
{
    /// <summary>
    /// Specifies the token information, used for authentication and token refreshing.
    /// </summary>
    public class TokenAuthResult
    {
        public DateTimeOffset? Expires { get; }

        /// <summary>
        /// Specifies the amount of time in seconds before the token expires.
        /// </summary>
        public int ExpiryTime { get; }

        /// <summary>
        /// Specifies the token to be used for authentication.
        /// </summary>
        public string Token { get; }

        /// <param name="token">The token to be used for authentication.</param>
        /// <param name="expiryTime">The amount of time in seconds before the token expires.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <remarks>An expiry time of 0 specifies that the token does not expire.</remarks>
        public TokenAuthResult(string token, int expiryTime = 0)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;
            ExpiryTime = expiryTime;

            if (ExpiryTime > 0)
            {
                Expires = DateTimeOffset.Now + TimeSpan.FromSeconds(expiryTime);
            }
        }
    }
}