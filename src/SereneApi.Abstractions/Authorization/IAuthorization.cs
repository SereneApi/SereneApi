namespace SereneApi.Abstractions.Authorization
{
    /// <summary>
    /// Authorizes an API request.
    /// </summary>
    public interface IAuthorization
    {
        /// <summary>
        /// Specifies the authorization scheme.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Specifies the authorization parameter.
        /// </summary>
        string Parameter { get; }
    }
}
