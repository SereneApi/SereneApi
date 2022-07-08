using DeltaWare.SDK.Core.Validators;
using System;
using System.Collections.Generic;

namespace SereneApi.Authentication.Web.Msal.Options
{
    internal sealed class MsalAuthenticationOptions : IMsalAuthenticationOptions, IMsalClientAuthentication
    {
        public string AppScope { get; private set; }
        public string ClientId { get; }
        public List<string> UserScopes { get; set; } = new();

        public MsalAuthenticationOptions(string clientId)
        {
            StringValidator.ThrowOnNullOrWhitespace(clientId, nameof(clientId));

            ClientId = clientId;
        }

        public IMsalUserScopes EnableClientAuthentication()
        {
            AppScope = $"api://{ClientId}/.default";

            return this;
        }

        public IMsalUserScopes RegisterUserScope(string scope)
        {
            StringValidator.ThrowOnNullOrWhitespace(scope, nameof(scope));

            UserScopes.Add($"api://{ClientId}/{scope}");

            return this;
        }

        public IMsalUserScopes RegisterUserScopes(IEnumerable<string> scopes)
        {
            if (scopes == null)
            {
                throw new ArgumentNullException(nameof(scopes));
            }

            foreach (string scope in scopes)
            {
                RegisterUserScope(scope);
            }

            return this;
        }
    }
}