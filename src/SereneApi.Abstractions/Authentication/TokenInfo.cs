namespace SereneApi.Abstractions.Authentication
{
    public class TokenInfo
    {
        /// <summary>
        /// The Specific token to be used for authentication.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// The amount of time in seconds before the token expires.
        /// </summary>
        public int ExpiryTime { get; }

        /// <param name="token">The token to be used for authentication.</param>
        /// <param name="expiryTime">The amount of time in seconds before the token expires.</param>
        /// <remarks>An expiry time of 0 means the token does not expire.</remarks>
        public TokenInfo(string token, int expiryTime = 0)
        {
            Token = token;
            ExpiryTime = expiryTime;
        }
    }
}
