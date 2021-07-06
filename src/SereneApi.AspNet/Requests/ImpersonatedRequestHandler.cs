using DeltaWare.Dependencies.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Requests;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.AspNet.Requests
{
    [SupportedOSPlatform("windows")]
    public class ImpersonatedRequestHandler : DefaultRequestHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly ILogger _logger;

        public ImpersonatedRequestHandler(IDependencyProvider dependencies) : base(dependencies)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException("This class is only supported in windows");
            }

            _contextAccessor = dependencies.GetDependency<IHttpContextAccessor>();

            dependencies.TryGetDependency(out _logger);
        }

        protected override async Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, Uri route, Method method, IRequestContent content, CancellationToken cancellationToken = default)
        {
            if (_contextAccessor.HttpContext.User.Identity is WindowsIdentity windowsIdentity)
            {
                _logger.LogDebug("Impersonating the request as {UserName}", windowsIdentity.Name);

                return await WindowsIdentity.RunImpersonated(windowsIdentity.AccessToken, async () =>
                    await base.HandleRequestAsync(client, route, method, content, cancellationToken));
            }

            _logger.LogWarning("Request will not be impersonated as no Windows Identity was found");

            return await base.HandleRequestAsync(client, route, method, content, cancellationToken);
        }

        protected override async Task<HttpResponseMessage> HandleRequestAsync(HttpClient client, Uri route, Method method, CancellationToken cancellationToken = default)
        {
            if (_contextAccessor.HttpContext.User.Identity is WindowsIdentity windowsIdentity)
            {
                _logger.LogDebug("Impersonating the request as {UserName}", windowsIdentity.Name);

                return await WindowsIdentity.RunImpersonated(windowsIdentity.AccessToken, async () =>
                    await base.HandleRequestAsync(client, route, method, cancellationToken));
            }

            _logger.LogWarning("Request will not be impersonated as no Windows Identity was found");

            return await base.HandleRequestAsync(client, route, method, cancellationToken);
        }
    }
}
