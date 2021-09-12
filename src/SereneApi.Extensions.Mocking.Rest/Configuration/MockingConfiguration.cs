using DeltaWare.Dependencies.Abstractions;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Responses;
using SereneApi.Extensions.Mocking.Rest.Responses.Factories;
using SereneApi.Extensions.Mocking.Rest.Responses.Manager;
using System;
using System.Collections.Generic;

namespace SereneApi.Extensions.Mocking.Rest.Configuration
{
    internal class MockingConfiguration : IMockingConfiguration
    {
        private readonly IDependencyProvider _dependencies;

        private readonly List<IMockResponseDescriptor> _mockResponses = new();

        public MockingConfiguration(IDependencyProvider dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        }

        public IMockResponseManager BuildManager(ISerializer serializer)
        {
            return new MockResponseManager(_mockResponses, serializer);
        }

        public IMockResponseFactory RegisterMockResponse()
        {
            return new MockResponseFactory(this, _dependencies.GetDependency<ISerializer>());
        }

        public void RegisterMockResponse(IMockResponseDescriptor descriptor)
        {
            _mockResponses.Add(descriptor);
        }
    }
}