using SereneApi.Extensions.Mocking.Helpers;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Extensions.Mocking.Types.Dependencies;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using SereneApi.Interfaces.Requests;

namespace SereneApi.Extensions.Mocking.Types
{
    /// <inheritdoc cref="IMockResponseExtensions"/>
    public class MockResponseExtensions: CoreOptions, IMockResponseExtensions
    {
        private readonly ISerializer _serializer;

        public MockResponseExtensions(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
            _serializer = DependencyCollection.GetDependency<ISerializer>();
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public IMockResponseExtensions RespondsToRequestsWith(params string[] uris)
        {
            ExceptionHelper.EnsureArrayIsNotEmpty(uris, nameof(uris));

            if(DependencyCollection.HasDependency<RouteWhitelistDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            List<Uri> routeWhitelist = uris.Select(uri => new Uri(uri)).ToList();

            DependencyCollection.AddDependency(new RouteWhitelistDependency(routeWhitelist));

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public IMockResponseExtensions RespondsToRequestsWith<TContent>(TContent inBodyContent)
        {
            ExceptionHelper.EnsureParameterIsNotNull(inBodyContent, nameof(inBodyContent));

            IApiRequestContent content = _serializer.Serialize(inBodyContent);

            if(DependencyCollection.TryGetDependency(out ContentWhitelistDependency contentWhitelist))
            {
                contentWhitelist.ExtendWhitelist(content);
            }
            else
            {
                DependencyCollection.AddDependency(new ContentWhitelistDependency(content));
            }

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public IMockResponseExtensions RespondsToRequestsWith(Method method)
        {
            ExceptionHelper.EnsureCorrectMethod(method);

            if(DependencyCollection.HasDependency<MethodWhitelistDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency(new MethodWhitelistDependency(method));

            return this;
        }

        /// <inheritdoc cref="IMockResponseExtensions.ResponseIsDelayed"/>
        public IMockResponseExtensions ResponseIsDelayed(int seconds, int delayCount = 0)
        {
            if(DependencyCollection.HasDependency<DelayedResponseDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency(new DelayedResponseDependency(seconds, delayCount));

            return this;
        }
    }
}
