using SereneApi.Extensions.Mocking.Helpers;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Extensions.Mocking.Types.Dependencies;
using SereneApi.Interfaces;
using SereneApi.Interfaces.Requests;
using SereneApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Types
{
    /// <inheritdoc cref="IMockResponseExtensions"/>
    public class MockResponseExtensions: CoreOptions, IMockResponseExtensions
    {
        private readonly ISerializer _serializer;

        public MockResponseExtensions(DependencyCollection dependencies) : base(dependencies)
        {
            _serializer = Dependencies.GetDependency<ISerializer>();
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public IMockResponseExtensions RespondsToRequestsWith(params string[] uris)
        {
            ExceptionHelper.EnsureArrayIsNotEmpty(uris, nameof(uris));

            if(Dependencies.HasDependency<RouteWhitelistDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            List<Uri> routeWhitelist = uris.Select(uri => new Uri(uri)).ToList();

            Dependencies.AddDependency(new RouteWhitelistDependency(routeWhitelist));

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public IMockResponseExtensions RespondsToRequestsWith<TContent>(TContent inBodyContent)
        {
            ExceptionHelper.EnsureParameterIsNotNull(inBodyContent, nameof(inBodyContent));

            IApiRequestContent content = _serializer.Serialize(inBodyContent);

            if(Dependencies.TryGetDependency(out ContentWhitelistDependency contentWhitelist))
            {
                contentWhitelist.ExtendWhitelist(content);
            }
            else
            {
                Dependencies.AddDependency(new ContentWhitelistDependency(content));
            }

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public IMockResponseExtensions RespondsToRequestsWith(Method method)
        {
            ExceptionHelper.EnsureCorrectMethod(method);

            if(Dependencies.HasDependency<MethodWhitelistDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency(new MethodWhitelistDependency(method));

            return this;
        }

        /// <inheritdoc cref="IMockResponseExtensions.ResponseIsDelayed"/>
        public IMockResponseExtensions ResponseIsDelayed(int seconds, int delayCount = 0)
        {
            if(Dependencies.HasDependency<DelayedResponseDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            Dependencies.AddDependency(new DelayedResponseDependency(seconds, delayCount));

            return this;
        }
    }
}
