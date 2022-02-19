using SereneApi.Authentication.WebAssembly.Msal.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Authorization;
using SereneApi.Core.Http.Authorization.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SereneApi.Authentication.WebAssembly.Msal
{
    internal class MsalTokenAuthenticator : IAuthenticator
    {
        private readonly IMsalAuthenticationOptions _authenticationOptions;
        private readonly ILogger _logger;
        private readonly NavigationManager _navigation;
        private readonly IAccessTokenProvider _provider;
        private IAuthorization _cachedAuthorization;
        private AccessToken _lastToken;

        public MsalTokenAuthenticator(IAccessTokenProvider provider, NavigationManager navigation, IMsalAuthenticationOptions authenticationOptions = null, ILogger logger = null)
        {
            _provider = provider;
            _navigation = navigation;
            _authenticationOptions = authenticationOptions;
            _logger = logger;
        }

        public async Task<IAuthorization> AuthorizeAsync()
        {
            if (_lastToken != null && DateTimeOffset.Now < _lastToken.Expires.AddMinutes(-5.0))
            {
                return _cachedAuthorization;
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
            _cachedAuthorization = new BearerAuthorization(_lastToken.Value);

            return _cachedAuthorization;
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