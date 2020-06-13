using SereneApi.Extensions.Mocking.Helpers;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Extensions.Mocking.Types.Dependencies;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Types
{
    public class MockResponseExtensions : CoreOptions, IMockResponseExtensions
    {
        private readonly ISerializer _serializer;

        public MockResponseExtensions(DependencyCollection dependencyCollection) : base(dependencyCollection)
        {
            _serializer = DependencyCollection.GetDependency<ISerializer>();
        }

        public IMockResponseExtensions RespondsToRequestsWith(params string[] uris)
        {
            if (uris == null || uris.Length <= 0)
            {
                throw new ArgumentNullException(nameof(uris));
            }

            if (DependencyCollection.HasDependency<RouteWhitelistDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            List<Uri> routeWhitelist = uris.Select(uri => new Uri(uri)).ToList();

            DependencyCollection.AddDependency(new RouteWhitelistDependency(routeWhitelist));

            return this;
        }

        public IMockResponseExtensions RespondsToRequestsWith<TContent>(TContent inBodyContent)
        {
            if (inBodyContent == null)
            {
                throw new ArgumentNullException(nameof(inBodyContent));
            }

            IApiRequestContent content = _serializer.Serialize(inBodyContent);

            if (DependencyCollection.TryGetDependency(out ContentWhitelistDependency contentWhitelist))
            {
                contentWhitelist.ExtendWhitelist(content);
            }
            else
            {
                DependencyCollection.AddDependency(new ContentWhitelistDependency(content));
            }

            return this;
        }

        public IMockResponseExtensions RespondsToRequestsWith(Method method)
        {
            if (method == Method.None)
            {
                throw new ArgumentException("Invalid Method provided", nameof(method));
            }

            if (DependencyCollection.HasDependency<MethodWhitelistDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency(new MethodWhitelistDependency(method));

            return this;
        }

        public IMockResponseExtensions ResponseIsDelayed(int seconds, int delayCount = 0)
        {
            if (DependencyCollection.HasDependency<DelayResponseDependency>())
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            DependencyCollection.AddDependency(new DelayResponseDependency(seconds, delayCount));

            return this;
        }
    }
}
