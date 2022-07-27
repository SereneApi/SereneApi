using SereneApi.Core.Http.Authentication;
using System;
using System.Text;

namespace SereneApi.Core.Http.Authorization.Types
{
    /// <summary>
    /// Authorizes an API request using Basic authentication with the specified Username and Password.
    /// </summary>
    public class BasicAuthentication : IAuthentication
    {
        /// <inheritdoc cref="IAuthentication.Parameter"/>
        public string Parameter { get; }

        /// <inheritdoc cref="IAuthentication.Scheme"/>
        public string Scheme { get; } = "Basic";

        /// <summary>
        /// Creates a new instance of <see cref="BasicAuthentication"/>.
        /// </summary>
        /// <param name="username">The username to be authorized with.</param>
        /// <param name="password">The password to be authorized with.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="EncoderFallbackException"></exception>
        public BasicAuthentication(string username, string password)
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