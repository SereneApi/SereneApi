namespace SereneApi.Abstractions.Authorization
{
    /// <summary>
    /// Authenticates an API request.
    /// </summary>
    public interface IAuthorization
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
