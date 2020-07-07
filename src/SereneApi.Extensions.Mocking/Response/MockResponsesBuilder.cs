using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Request.Content;
using SereneApi.Abstractions.Response;
using SereneApi.Abstractions.Serializers;
using SereneApi.Extensions.Mocking.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SereneApi.Extensions.Mocking.Response
{
    // This type should not be disposed of as it does not own the DependencyCollection.
    /// <inheritdoc cref="IMockResponsesBuilder"/>
    public class MockResponsesBuilder: IMockResponsesBuilder
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
        public IMockResponseExtensions AddMockResponse(Status status, string message = null)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _dependencies.GetDependency<ISerializer>(), Binding.Unbound);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), status, message, null));

            return BuildResponseExtensions(dependencies);
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseExtensions AddMockResponse<TContent>(TContent content)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _dependencies.GetDependency<ISerializer>(), Binding.Unbound);

            IApiRequestContent responseContent = _dependencies.GetDependency<ISerializer>().Serialize(content);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), Status.Ok, null, responseContent));

            return BuildResponseExtensions(dependencies);
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseExtensions AddMockResponse<TContent>(TContent content, Status status)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _dependencies.GetDependency<ISerializer>(), Binding.Unbound);

            IApiRequestContent responseContent = _dependencies.GetDependency<ISerializer>().Serialize(content);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), status, null, responseContent));

            return BuildResponseExtensions(dependencies);
        }

        protected virtual IMockResponseExtensions BuildResponseExtensions(IDependencyCollection dependencies)
        {
            return new MockResponseExtensions(dependencies);
        }

        protected virtual IMockResponse BuildMockResponse(IDependencyProvider provider, Status status, string message, IApiRequestContent responseContent)
        {
            return new MockResponse(provider, status, message, responseContent);
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
