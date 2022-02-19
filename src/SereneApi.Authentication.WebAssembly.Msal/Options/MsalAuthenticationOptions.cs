using DeltaWare.SDK.Core.Validators;
using System;
using System.Collections.Generic;

namespace SereneApi.Authentication.WebAssembly.Msal.Options
{
    internal class MsalAuthenticationOptions : IMsalAuthenticationOptionsBuilder, IMsalAuthenticationOptions
    {
        public string ClientId { get; }

        public string ReturnUrl { get; set; }
        public List<string> Scopes { get; set; } = new();

        public MsalAuthenticationOptions(string clientId)
        {
            StringValidator.ThrowOnNullOrWhitespace(clientId, nameof(clientId));

            ClientId = clientId;
        }

        public void RegisterScope(string scope)
        {
            StringValidator.ThrowOnNullOrWhitespace(scope, nameof(scope));

            Scopes.Add($"api://{ClientId}/{scope}");
        }

        public void RegisterScopes(IEnumerable<string> scopes)
        {
            if (scopes == null)
            {
                throw new NullReferenceException(nameof(scopes));
            }

            foreach (string scope in scopes)
            {
                RegisterScope(scope);
            }
        }
    }
}