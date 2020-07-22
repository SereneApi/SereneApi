using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Authorization.Authorizers
{
    /// <summary>
    /// Specifies the token information, used for authorization and token refreshing.
    /// </summary>
    public class TokenAuthResult
    {
        /// <summary>
        /// Specifies the token to be used for authorization.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Specifies the amount of time in seconds before the token expires.
        /// </summary>
        public int ExpiryTime { get; }

        /// <param name="token">The token to be used for authorization.</param>
        /// <param name="expiryTime">The amount of time in seconds before the token expires.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        /// <remarks>An expiry time of 0 specifies that the token does not expire.</remarks>
        public TokenAuthResult([NotNull] string token, int expiryTime = 0)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;
            ExpiryTime = expiryTime;
        }
    }
}
