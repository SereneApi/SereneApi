using SereneApi.Core.Http.Authentication;
using System;

namespace SereneApi.Core.Http.Authorization.Types
{
    /// <summary>
    /// Authorizes an API request using Bearer authentication.
    /// </summary>
    public class BearerAuthentication : IAuthentication
    {
        /// <inheritdoc cref="IAuthentication.Parameter"/>
        public string Parameter { get; }

        /// <inheritdoc cref="IAuthentication.Scheme"/>
        public string Scheme { get; } = "Bearer";

        /// <summary>
        /// Creates a new instance of <see cref="BearerAuthentication"/>.
        /// </summary>
        /// <param name="token">The token to be used for Bearer authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public BearerAuthentication(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Parameter = token;
        }
    }
}