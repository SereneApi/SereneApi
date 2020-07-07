using SereneApi.Abstractions.Request;

namespace SereneApi.Abstractions.Authentication
{
    /// <summary>
    /// Authenticates an <see cref="IApiRequest"/> using a token.
    /// </summary>
    public class BearerAuthentication: IAuthentication
    {
        /// <inheritdoc cref="IAuthentication.Scheme"/>
        public string Scheme { get; } = "Bearer";

        /// <inheritdoc cref="IAuthentication.Parameter"/>
        public string Parameter { get; }

        /// <param name="token">The token to be used for bearer authentication.</param>
        public BearerAuthentication(string token)
        {
            Parameter = token;
        }
    }
}
