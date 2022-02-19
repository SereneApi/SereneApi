using System.Collections.Generic;

namespace SereneApi.Authentication.WebAssembly.Msal.Options
{
    public interface IMsalAuthenticationOptions
    {
        string ClientId { get; }
        string ReturnUrl { get; }
        List<string> Scopes { get; }
    }
}