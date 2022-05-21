using System.Collections.Generic;

namespace SereneApi.Authentication.WebAssembly.Msal.Options
{
    public interface IMsalAuthenticationOptions
    {
        /// <summary>
        /// Specified the ClientId of the application.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Specifies the Url to be returned to after authentication.
        /// </summary>
        string ReturnUrl { get; }

        /// <summary>
        /// Specifies the scopes to be authenticated against.
        /// </summary>
        List<string> Scopes { get; }
    }
}