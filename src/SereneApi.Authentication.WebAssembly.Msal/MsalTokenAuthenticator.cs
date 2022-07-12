using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using SereneApi.Authentication.WebAssembly.Msal.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Authorization.Types;
using System;
using System.Threading.Tasks;

namespace SereneApi.Authentication.WebAssembly.Msal
{
    internal class MsalTokenAuthenticator : IAuthenticator
    {
        private readonly IMsalAuthenticationOptions _authenticationOptions;
        private readonly ILogger _logger;
        private readonly NavigationManager _navigation;
        private readonly IAccessTokenProvider _provider;
        private IAuthentication _cachedAuthentication;
        private AccessToken _lastToken;

        public MsalTokenAuthenticator(IAccessTokenProvider provider, NavigationManager navigation, IMsalAuthenticationOptions authenticationOptions = null, ILogger logger = null)
        {
            _provider = provider;
            _navigation = navigation;
            _authenticationOptions = authenticationOptions;
            _logger = logger;
        }

        public async Task<IAuthentication> AuthorizeAsync()
        {
            if (_lastToken != null && DateTimeOffset.Now < _lastToken.Expires.AddMinutes(-5.0))
            {
                return _cachedAuthentication;
            }

            AccessTokenResult accessTokenResult;

            if (_authenticationOptions != null)
            {
                _logger.LogInformation("Retrieving Access Token for Client {id}", _authenticationOptions.ClientId);

                accessTokenResult = await _provider.RequestAccessToken(BuildRequestOptions());
            }
            else
            {
                _logger.LogInformation("Retrieving Access Token for user");

                accessTokenResult = await _provider.RequestAccessToken();
            }

            if (!accessTokenResult.TryGetToken(out AccessToken accessToken))
            {
                throw new AccessTokenNotAvailableException(_navigation, accessTokenResult, _authenticationOptions?.Scopes);
            }

            _logger.LogTrace("Retrieved Bearer token [{token}]", accessToken.Value);

            _lastToken = accessToken;
            _cachedAuthentication = new BearerAuthentication(_lastToken.Value);

            return _cachedAuthentication;
        }

        private AccessTokenRequestOptions BuildRequestOptions()
        {
            return new AccessTokenRequestOptions
            {
                Scopes = _authenticationOptions.Scopes,
                ReturnUrl = _authenticationOptions.ReturnUrl
            };
        }
    }
}