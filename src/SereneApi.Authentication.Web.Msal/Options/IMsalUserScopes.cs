using System.Collections.Generic;

namespace SereneApi.Authentication.Web.Msal.Options
{
    public interface IMsalUserScopes
    {
        /// <summary>
        /// Registers an API authentication scope against this API to be used by the user.
        /// </summary>
        IMsalUserScopes RegisterScope(string scope);

        /// <summary>
        /// Registers API authentication scopes against this API to be used by the user.
        /// </summary>
        IMsalUserScopes RegisterScopes(IEnumerable<string> scopes);
    }
}