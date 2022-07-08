using System.Collections.Generic;

namespace SereneApi.Authentication.Web.Msal.Options
{
    public interface IMsalAuthenticationOptions
    {
        /// <summary>
        /// Specifies the applications scope.
        /// </summary>
        string AppScope { get; }

        /// <summary>
        /// Specifies the user scopes to be authenticated.
        /// </summary>
        List<string> UserScopes { get; }
    }
}