using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SereneApi.Authentication.Web.IdentityServer.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Authorization.Types;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Authentication.Web.IdentityServer
{
    public class IdentityServerAuthenticator : IAuthenticator
    {
        protected IAuthenticationOptions Options { get; }

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IMemoryCache? _tokenCache;

        private readonly ILogger? _logger;

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

        public IdentityServerAuthenticator(IAuthenticationOptions options, IHttpContextAccessor contextAccessor, IMemoryCache? tokenCache = null, ILogger? logger = null)
        {
            Options = options;
            _contextAccessor = contextAccessor;
            _tokenCache = tokenCache;
            _logger = logger;
        }

        public async Task<IAuthentication> AuthorizeAsync()
        {
            string? userToken = null;

            if (Options.EnableClientDelegation)
            {
                userToken = await GetUserTokenAsync();
            }

            string? accessToken;

            if (string.IsNullOrWhiteSpace(userToken))
            {
                _logger?.LogTrace("Using Client Authentication");

                if (_tokenCache == null || !_tokenCache.TryGetValue(Options.ClientId, out accessToken))
                {
                    accessToken = await InternalRenewClientTokenAsync();
                }
            }
            else
            {
                _logger?.LogTrace("Using User Authentication");

                if (_tokenCache == null || !_tokenCache.TryGetValue(userToken, out accessToken))
                {
                    accessToken = await InternalRenewDelegateTokenAsync(userToken);
                }
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException("No access token could be retrieved.");
            }

            OnTokenRetrieved(accessToken);

            return new BearerAuthentication(accessToken);
        }

        protected virtual async Task<string?> GetUserTokenAsync()
        {
            string? userToken = null;

            if (_contextAccessor.HttpContext != null)
            {
                userToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

                if (string.IsNullOrWhiteSpace(userToken))
                {
                    _logger?.LogDebug($"No user token could be retrieved from the {nameof(HttpContext)}");
                }
                else
                {
                    _logger?.LogTrace("User token was successfully retrieved.");
                }
            }
            else
            {
                _logger?.LogWarning($"Client Delegation is enabled, but no instance of {nameof(HttpContext)} could be found.");
            }

            return userToken;
        }

        private async Task<string?> InternalRenewDelegateTokenAsync(string token)
        {
            SemaphoreSlim locker = Locks.GetOrAdd(token, new SemaphoreSlim(1, 1));

            if (!await locker.WaitAsync(Options.Timeout))
            {
                return null;
            }

            try
            {
                TokenResponse tokenResponse = await RenewDelegateTokenAsync(token);

                if (tokenResponse.IsError)
                {
                    _logger?.LogWarning("Delegation authorization failed. {Error} {ErrorDescription}", tokenResponse.Error, tokenResponse.ErrorDescription);

                    throw new UnauthorizedAccessException(tokenResponse.Error);
                }

                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn),
                    Priority = CacheItemPriority.Normal
                };

                _tokenCache?.Set(token, tokenResponse.AccessToken, cacheExpirationOptions);

                return tokenResponse.AccessToken;
            }
            finally
            {
                locker.Release();

                Locks.TryRemove(token, out _);
            }
        }

        protected virtual async Task<TokenResponse> RenewDelegateTokenAsync(string token)
        {
            HttpClient client = new HttpClient();

            try
            {
                return await client.RequestTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = $"{Options.Authority}/connect/token",
                    ClientId = Options.ClientId,
                    ClientSecret = Options.ClientSecret,
                    GrantType = Options.DelegationGrantType,
                    Parameters = new Parameters
                    {
                        { "token", token }
                    }
                });
            }
            finally
            {
                client.Dispose();
            }
        }

        private async Task<string?> InternalRenewClientTokenAsync()
        {
            SemaphoreSlim locker = Locks.GetOrAdd(Options.ClientId, new SemaphoreSlim(1, 1));

            if (!await locker.WaitAsync(Options.Timeout))
            {
                return null;
            }

            try
            {
                TokenResponse tokenResponse = await RenewClientTokenAsync();

                if (tokenResponse.IsError)
                {
                    _logger?.LogWarning("Client authorization failed. {Error} {ErrorDescription}", tokenResponse.Error, tokenResponse.ErrorDescription);

                    throw new UnauthorizedAccessException(tokenResponse.Error);
                }

                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn),
                    Priority = CacheItemPriority.Normal
                };

                _tokenCache?.Set(Options.ClientId, tokenResponse.AccessToken, cacheExpirationOptions);

                return tokenResponse.AccessToken;
            }
            finally
            {
                locker.Release();

                Locks.TryRemove(Options.ClientId, out _);
            }
        }

        protected virtual async Task<TokenResponse> RenewClientTokenAsync()
        {
            HttpClient client = new HttpClient();

            try
            {
                return await client.RequestTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = $"{Options.Authority}/connect/token",
                    ClientId = Options.ClientId,
                    ClientSecret = Options.ClientSecret,
                    GrantType = Options.ClientGrantType
                });
            }
            finally
            {
                client.Dispose();
            }
        }
        
        protected virtual void OnTokenRetrieved(string accessToken)
        {
        }
    }
}
