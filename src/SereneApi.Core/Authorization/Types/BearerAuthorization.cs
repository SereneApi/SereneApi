using System;

namespace SereneApi.Core.Authorization.Types
{
    /// <summary>
    /// Authorizes an API request using Bearer authorization.
    /// </summary>
    public class BearerAuthorization : IAuthorization
    {
        /// <inheritdoc cref="IAuthorization.Parameter"/>
        public string Parameter { get; }

        /// <inheritdoc cref="IAuthorization.Scheme"/>
        public string Scheme { get; } = "Bearer";

        /// <summary>
        /// Creates a new instance of <see cref="BearerAuthorization"/>.
        /// </summary>
        /// <param name="token">The token to be used for Bearer authorization.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public BearerAuthorization(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Parameter = token;
        }
    }
}