using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SereneApi.Core.Events;
using SereneApi.Core.Http;
using SereneApi.Core.Http.Client;
using SereneApi.Core.Http.Requests;
using SereneApi.Core.Http.Requests.Handler;
using SereneApi.Core.Http.Responses.Handlers;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.WindowsImpersonation
{
    [SupportedOSPlatform("windows")]
    internal class ImpersonatingRequestHandler : RetryingRequestHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly ILogger _logger;

        public ImpersonatingRequestHandler(IHttpContextAccessor contextAccessor, IClientFactory clientFactory, IConnectionSettings connection, IResponseHandler responseHandler, IEventManager eventManager = null, ILogger logger = null) : base(clientFactory, connection, responseHandler, eventManager, logger)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This class is only supported on the Windows platform");
            }

            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, IApiRequest request, CancellationToken cancellationToken = default)
        {
            if (_contextAccessor.HttpContext.User.Identity is WindowsIdentity windowsIdentity)
            {
                _logger?.LogDebug("Impersonating the request as {UserName}", windowsIdentity.Name);

                // Running the async version of RunImpersonated always throws a socket exception.
                // Thus this request needs to be ran synchronously.
                return WindowsIdentity.RunImpersonated(windowsIdentity.AccessToken, () =>
                    base.HandleRequestAsync(client, request, cancellationToken).GetAwaiter().GetResult());
            }

            _logger?.LogDebug("Request will not be impersonated as no Windows Identity was found");

            return await base.HandleRequestAsync(client, request, cancellationToken);
        }
    }
}