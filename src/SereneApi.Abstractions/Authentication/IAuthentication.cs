namespace SereneApi.Abstractions.Authentication
{
    /// <summary>
    /// Authenticates an API request.
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Specifies the authentication scheme.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Specifies the authentication parameter.
        /// </summary>
        string Parameter { get; }
    }
}
