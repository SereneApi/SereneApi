using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SereneApi.Authentication.Web.IdentityServer.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Authorization.Types;

namespace SereneApi.Authentication.Web.IdentityServer
{
    public class IdentityServerAuthenticator : IAuthenticator
    {
        protected IAuthenticationOptions Options { get; }

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IMemoryCache? _cache;

        private readonly ILogger? _logger;

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

        public IdentityServerAuthenticator(IAuthenticationOptions options, IHttpContextAccessor contextAccessor, IMemoryCache? cache = null, ILogger? logger = null)
        {
            Options = options;
            _contextAccessor = contextAccessor;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IAuthentication> AuthorizeAsync()
        {
            string? userToken = null;

            if (Options.EnableClientDelegation)
            {
                if (_contextAccessor.HttpContext != null)
                {
                    userToken = await _contextAccessor.HttpContext.GetTokenAsync("access_token");

                    if (string.IsNullOrWhiteSpace(userToken))
                    {
                        _logger?.LogDebug($"No user token could be retrieved from the {nameof(HttpContext)}");
                    }
                }
                else
                {
                    _logger?.LogWarning($"Client Delegation is enabled, but no instance of {nameof(HttpContext)} could be found.");
                }
            }

            string? bearerToken;

            if (string.IsNullOrWhiteSpace(userToken))
            {
                _logger?.LogTrace("Using Client Authentication");

                if (_cache == null || !_cache.TryGetValue(Options.ClientId, out bearerToken))
                {
                    bearerToken = await InternalRenewClientTokenAsync();
                }
            }
            else
            {
                _logger?.LogTrace("Using User Authentication");

                if (_cache == null || !_cache.TryGetValue(userToken, out bearerToken))
                {
                    bearerToken = await InternalRenewDelegateTokenAsync(userToken);
                }
            }

            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                throw new UnauthorizedAccessException("No bearer token could be retrieved.");
            }

            return new BearerAuthentication(bearerToken);
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
                    throw new UnauthorizedAccessException(tokenResponse.Error);
                }

                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn),
                    Priority = CacheItemPriority.Normal
                };

                _cache?.Set(token, tokenResponse.AccessToken, cacheExpirationOptions);

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
                    GrantType = "delegation",
                    Parameters = new Parameters
                    {
                        {"token", token}
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
                    throw new UnauthorizedAccessException(tokenResponse.Error);
                }

                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn),
                    Priority = CacheItemPriority.Normal
                };

                _cache?.Set(Options.ClientId, tokenResponse.AccessToken, cacheExpirationOptions);

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
                    GrantType = "client_credentials",
                });
            }
            finally
            {
                client.Dispose();
            }

        }
    }
}
