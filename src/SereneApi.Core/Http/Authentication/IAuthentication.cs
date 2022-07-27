namespace SereneApi.Core.Http.Authentication
{
    /// <summary>
    /// Authorizes an API request.
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Specifies the authentication parameter.
        /// </summary>
        string Parameter { get; }

        /// <summary>
        /// Specifies the authentication scheme.
        /// </summary>
        string Scheme { get; }
    }
}