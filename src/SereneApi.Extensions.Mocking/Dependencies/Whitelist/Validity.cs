namespace SereneApi.Extensions.Mocking.Dependencies.Whitelist
{
    /// <summary>
    /// Used in conjunction with <see cref="IWhitelist"/> to weigh the response.
    /// </summary>
    public enum Validity
    {
        /// <summary>
        /// Used if the object being verified is not applicable. For example types do not match.
        /// </summary>
        NotApplicable,
        /// <summary>
        /// A complete match.
        /// </summary>
        Valid,
        /// <summary>
        /// Does not match.
        /// </summary>
        Invalid
    }
}
