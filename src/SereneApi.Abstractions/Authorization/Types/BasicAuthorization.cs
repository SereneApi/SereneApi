using System;
using System.Text;

namespace SereneApi.Abstractions.Authorization.Types
{
    /// <summary>
    /// Authorizes an API request using Basic authorization with the specified Username and Password.
    /// </summary>
    public class BasicAuthorization : IAuthorization
    {
        /// <inheritdoc cref="IAuthorization.Scheme"/>
        public string Scheme { get; } = "Basic";

        /// <inheritdoc cref="IAuthorization.Parameter"/>
        public string Parameter { get; }

        /// <summary>
        /// Creates a new instance of <see cref="BasicAuthorization"/>.
        /// </summary>
        /// <param name="username">The username to be authorized with.</param>
        /// <param name="password">The password to be authorized with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="EncoderFallbackException"></exception>
        public BasicAuthorization(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");

            Parameter = Convert.ToBase64String(byteArray);
        }
    }
}
