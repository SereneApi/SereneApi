using SereneApi.Abstractions.Request;
using System;
using System.Text;

namespace SereneApi.Abstractions.Authentication
{
    /// <summary>
    /// Authenticates an <see cref="IApiRequest"/> using a username and password.
    /// </summary>
    public class BasicAuthentication: IAuthentication
    {
        /// <inheritdoc cref="IAuthentication.Scheme"/>
        public string Scheme { get; } = "Basic";

        /// <inheritdoc cref="IAuthentication.Parameter"/>
        public string Parameter { get; }

        /// <param name="username">The Username to be used for authentication.</param>
        /// <param name="password">The Password to be used for authentication.</param>
        public BasicAuthentication(string username, string password)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");

            Parameter = Convert.ToBase64String(byteArray);
        }
    }
}
