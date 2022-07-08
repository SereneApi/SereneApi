namespace SereneApi.Authentication.WebAssembly.Msal.Options
{
    public interface IMsalAuthenticationOptionsBuilder
    {
        /// <summary>
        /// Specifies the Url to be returned to after authentication is completed.
        /// </summary>
        string ReturnUrl { get; set; }

        /// <summary>
        /// Registers a scope to authenticate against.
        /// </summary>
        /// <remarks>The following template will be applied <strong>api://{ClientId}/{scope}</strong></remarks>
        /// <param name="scope">The scope to be authenticated against.</param>
        void RegisterScope(string scope);
    }
}