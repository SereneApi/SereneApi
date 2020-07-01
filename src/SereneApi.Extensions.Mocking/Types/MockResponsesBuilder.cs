using DeltaWare.Dependencies;
using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking.Helpers;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Interfaces.Requests;
using SereneApi.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using SereneApi.Abstractions.Enums;

namespace SereneApi.Extensions.Mocking.Types
{
    // This type should not be disposed of as it does not own the DependencyCollection.
    /// <inheritdoc cref="IMockResponsesBuilder"/>
    public class MockResponsesBuilder: IMockResponsesBuilder
    {
        private ISerializer _serializer = new JsonSerializer();

        private readonly List<Func<IMockResponse>> _mockResponses = new List<Func<IMockResponse>>();

        /// <inheritdoc cref="IMockResponsesBuilder"/>
        public void UseSerializer(ISerializer serializer)
        {
            ExceptionHelper.EnsureParameterIsNotNull(serializer, nameof(serializer));

            if(_serializer != null)
            {
                ExceptionHelper.MethodCannotBeCalledTwice();
            }

            _serializer = serializer;
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseExtensions AddMockResponse(Status status, string message = null)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _serializer, Binding.Unbound);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), status, message, null));

            return BuildResponseExtensions(dependencies);
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseExtensions AddMockResponse<TContent>(TContent content)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _serializer, Binding.Unbound);

            IApiRequestContent responseContent = _serializer.Serialize(content);

            _mockResponses.Add(() => BuildMockResponse(dependencies.BuildProvider(), Status.Ok, null, responseContent));

            return BuildResponseExtensions(dependencies);
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseExtensions AddMockResponse<TContent>(TContent content, Status status)
        {
            IDependencyCollection dependencies = new DependencyCollection();

            dependencies.AddScoped(() => _serializer, Binding.Unbound);

            IApiRequestContent responseContent = _serializer.Serialize(content);

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
