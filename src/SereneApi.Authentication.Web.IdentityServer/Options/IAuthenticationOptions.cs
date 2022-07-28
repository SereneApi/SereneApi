using System;

namespace SereneApi.Authentication.Web.IdentityServer.Options
{
    public interface IAuthenticationOptions
    {
        string ClientId { get; }

        string ClientSecret { get; }

        string Authority { get; }

        TimeSpan Timeout { get; }

        bool EnableClientDelegation { get; }
    }
}
