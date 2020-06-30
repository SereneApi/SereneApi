using DeltaWare.Dependencies;
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
        public MockResponseExtensions(IDependencyCollection dependencies) : base(dependencies)
        {
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

            Dependencies.AddScoped(() => new RouteWhitelistDependency(routeWhitelist));

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public IMockResponseExtensions RespondsToRequestsWith<TContent>(TContent inBodyContent)
        {
            ExceptionHelper.EnsureParameterIsNotNull(inBodyContent, nameof(inBodyContent));

            Dependencies.AddScoped(p =>
            {
                ISerializer serializer = p.GetDependency<ISerializer>();

                IApiRequestContent content = serializer.Serialize(inBodyContent);

                return new ContentWhitelistDependency(content);
            });

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

            Dependencies.AddScoped(() => new MethodWhitelistDependency(method));

            return this;
        }

        /// <inheritdoc cref="IMockResponseExtensions.ResponseIsDelayed"/>
        public IMockResponseExtensions ResponseIsDelayed(int seconds, int delayCount = 0)
        {
            if(Dependencies.HasDependency<DelayedResponseDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            // We want to keep this instance, so it is not created within the dependency.
            Dependencies.AddScoped(() =>
                new DelayedResponseDependency(seconds, delayCount));

            return this;
        }
    }
}
