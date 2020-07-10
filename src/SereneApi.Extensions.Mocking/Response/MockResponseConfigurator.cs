using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Request;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Serializers;
using SereneApi.Extensions.Mocking.Dependencies;
using SereneApi.Extensions.Mocking.Dependencies.Whitelist;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Response
{
    public class MockResponseConfigurator: IMockResponseConfigurator, ICoreOptions
    {
        public IDependencyCollection Dependencies { get; }

        /// <summary>
        /// Created a new instance of <see cref="MockResponseConfigurator"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">thrown if a null value is provided.</exception>
        public MockResponseConfigurator([NotNull] IDependencyCollection dependencies)
        {
            Dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }

        /// <inheritdoc cref="IMockResponseUrl.RespondsToRequestsWith"/>
        public virtual IMockResponseContent RespondsToRequestsWith(params string[] uris)
        {
            if(uris == null)
            {
                throw new ArgumentNullException(nameof(uris));
            }

            if(uris.Length == 0)
            {
                throw new ArgumentException($"{nameof(uris)} must not be Empty.");
            }

            Uri[] routeWhitelist = uris.Select(uri => new Uri(uri)).ToArray();

            Dependencies.AddScoped(() => new RouteWhitelistDependency(routeWhitelist));

            return this;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseExtensions.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public virtual void RespondsToRequestsWith<TContent>([NotNull] TContent inBodyContent)
        {
            if(inBodyContent == null)
            {
                throw new ArgumentNullException(nameof(inBodyContent));
            }

            Dependencies.AddScoped(p =>
            {
                ISerializer serializer = p.GetDependency<ISerializer>();

                IApiRequestContent content = serializer.Serialize(inBodyContent);

                return new ContentWhitelistDependency(content);
            });
        }

        /// <inheritdoc>
        ///     <cref>IMockResponseMethod.RespondsToRequestsWith</cref>
        /// </inheritdoc>
        public virtual IMockResponseUrl RespondsToRequestsWith([NotNull] params Method[] method)
        {
            Dependencies.AddScoped(() => new MethodWhitelistDependency(method));

            return this;
        }

        /// <inheritdoc cref="IMockResponseConfigurator.ResponseIsDelayed"/>
        public virtual IMockResponseMethod ResponseIsDelayed(int seconds, int delayCount = 0)
        {
            // We want to keep this instance, so it is not created within the dependency.
            Dependencies.AddScoped(() => new DelayedResponseDependency(seconds, delayCount));

            return this;
        }
    }
}
