using SereneApi.Abstractions.Request;

namespace SereneApi.Abstractions.Authentication
{
    /// <summary>
    /// Authenticates an <see cref="IApiRequest"/>
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// The Authentication Scheme.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// The Authentication Parameter.
        /// </summary>
        string Parameter { get; }
    }
}
