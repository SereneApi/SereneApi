using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Content;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Response
{
    // This type should not be disposed of as it does not own the DependencyCollection.
    /// <inheritdoc cref="IMockResponsesBuilder"/>
    public class MockResponsesBuilder : IMockResponsesBuilder
    {
        private readonly List<Func<IMockResponse>> _mockResponses = new List<Func<IMockResponse>>();

        private readonly IDependencyProvider _dependencies;

        public MockResponsesBuilder(IDependencyProvider dependencies)
        {
            _dependencies = dependencies;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseConfigurator AddMockResponse(Status status)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _dependencies.GetDependency<ISerializer>(), Binding.Unbound);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), status, null));

            return BuildResponseExtensions(dependencies);
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseConfigurator AddMockResponse<TContent>(TContent content)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _dependencies.GetDependency<ISerializer>(), Binding.Unbound);

            IRequestContent responseContent = _dependencies.GetDependency<ISerializer>().Serialize(content);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), Status.Ok, responseContent));

            return BuildResponseExtensions(dependencies);
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseConfigurator AddMockResponse<TContent>(TContent content, Status status)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _dependencies.GetDependency<ISerializer>(), Binding.Unbound);

            IRequestContent responseContent = _dependencies.GetDependency<ISerializer>().Serialize(content);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), status, responseContent));

            return BuildResponseExtensions(dependencies);
        }

        protected virtual IMockResponseConfigurator BuildResponseExtensions(IDependencyCollection dependencies)
        {
            return new MockResponseConfigurator(dependencies);
        }

        protected virtual IMockResponse BuildMockResponse(IDependencyProvider provider, Status status, IRequestContent responseContent)
        {
            return new MockResponse(provider, status, responseContent);
        }

        /// <summary>
        /// Builds the <see cref="IMockResponse"/>.
        /// </summary>
        public List<IMockResponse> Build()
        {
            return _mockResponses.Select(r => r.Invoke()).ToList();
        }
    }
}
