using System.Collections.Generic;

namespace SereneApi.Authentication.Web.Msal.Options
{
    public interface IMsalAuthenticationOptions
    {
        string AppScope { get; }
        List<string> UserScopes { get; }
    }
}