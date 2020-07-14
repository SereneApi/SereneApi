using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SereneApi.Abstractions.Authentication
{
    /// <summary>
    /// Authenticates an API request using Basic authentication with the specified Username and Password.
    /// </summary>
    public class BasicAuthentication: IAuthentication
    {
        /// <inheritdoc cref="IAuthentication.Scheme"/>
        public string Scheme { get; } = "Basic";

        /// <inheritdoc cref="IAuthentication.Parameter"/>
        public string Parameter { get; }

        /// <summary>
        /// Creates a new instance of <see cref="BasicAuthentication"/>.
        /// </summary>
        /// <param name="username">The username to be authenticated with.</param>
        /// <param name="password">The password to be authenticated with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="EncoderFallbackException"></exception>
        public BasicAuthentication([NotNull] string username, [NotNull] string password)
        {
            if(string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if(string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");

            Parameter = Convert.ToBase64String(byteArray);
        }
    }
}
