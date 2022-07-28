using System;

namespace SereneApi.Authentication.Web.IdentityServer.Options
{
    public sealed class AuthenticationOptions: IAuthenticationOptions
    {
        public string ClientId { get; set; } = null!;

        public string ClientSecret { get; set; } = null!;

        public string Authority { get; set; } = null!;
        
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        public bool EnableClientDelegation { get; set; }
    }
}