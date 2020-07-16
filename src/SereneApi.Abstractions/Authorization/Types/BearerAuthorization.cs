using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Authorization.Types
{
    /// <summary>
    /// Authenticates an API request using Bearer authentication.
    /// </summary>
    public class BearerAuthorization: IAuthorization
    {
        /// <inheritdoc cref="IAuthorization.Scheme"/>
        public string Scheme { get; } = "Bearer";

        /// <inheritdoc cref="IAuthorization.Parameter"/>
        public string Parameter { get; }

        /// <summary>
        /// Creates a new instance of <see cref="BearerAuthorization"/>.
        /// </summary>
        /// <param name="token">The token to be used for Bearer authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public BearerAuthorization([NotNull] string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Parameter = token;
        }
    }
}
