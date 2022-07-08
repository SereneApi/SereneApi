using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using SereneApi.Authentication.Web.Msal.Options;
using SereneApi.Core.Http.Authentication;
using SereneApi.Core.Http.Authorization;
using SereneApi.Core.Http.Authorization.Types;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SereneApi.Authentication.Web.Msal
{
    internal class MsalAuthenticator : IAuthenticator
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger _logger;
        private readonly IMsalAuthenticationOptions _options;
        private readonly ITokenAcquisition _tokenAcquisition;

        public MsalAuthenticator(ITokenAcquisition tokenAcquisition, IMsalAuthenticationOptions options, IHttpContextAccessor contextAccessor = null, ILogger logger = null)
        {
            _tokenAcquisition = tokenAcquisition;
            _options = options;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public async Task<IAuthorization> AuthorizeAsync()
        {
            string token;

            if (IsUserAuthenticated())
            {
                _logger.LogInformation("Retrieving access token for user.");

                token = await _tokenAcquisition.GetAccessTokenForUserAsync(_options.UserScopes);
            }
            else if (!string.IsNullOrWhiteSpace(_options.AppScope))
            {
                _logger.LogInformation("Retrieving access token for scope {scope}.", _options.AppScope);

                token = await _tokenAcquisition.GetAccessTokenForAppAsync(_options.AppScope);
            }
            else
            {
                throw new AuthenticationException("Could not authenticate the request as not user identity was present.", new ArgumentException("Use EnableClientAuthentication to circumvent this."));
            }

            _logger.LogTrace("Retrieved Bearer token [{token}]", token);

            return new BearerAuthorization(token);
        }

        private bool IsUserAuthenticated()
        {
            if (_contextAccessor?.HttpContext == null)
            {
                return false;
            }

            string userName = _contextAccessor.HttpContext.User.Identity?.Name;

            return !string.IsNullOrWhiteSpace(userName);
        }
    }
}