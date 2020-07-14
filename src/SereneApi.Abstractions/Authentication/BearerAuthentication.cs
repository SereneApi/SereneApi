using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Authentication
{
    /// <summary>
    /// Authenticates an API request using Bearer authentication.
    /// </summary>
    public class BearerAuthentication: IAuthentication
    {
        /// <inheritdoc cref="IAuthentication.Scheme"/>
        public string Scheme { get; } = "Bearer";

        /// <inheritdoc cref="IAuthentication.Parameter"/>
        public string Parameter { get; }

        /// <summary>
        /// Creates a new instance of <see cref="BearerAuthentication"/>.
        /// </summary>
        /// <param name="token">The token to be used for Bearer authentication.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        public BearerAuthentication([NotNull] string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            Parameter = token;
        }
    }
}
