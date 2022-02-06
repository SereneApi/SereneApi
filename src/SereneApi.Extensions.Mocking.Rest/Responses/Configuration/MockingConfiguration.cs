using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Responses.Factories;
using SereneApi.Extensions.Mocking.Rest.Responses.Manager;
using System;
using System.Collections.Generic;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Configuration
{
    internal class MockingConfiguration : IMockingConfiguration
    {
        private readonly List<IMockResponseDescriptor> _mockResponses = new();
        private readonly ISerializer _serializer;

        public MockingConfiguration(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IMockResponseManager BuildManager()
        {
            return new MockResponseManager(_mockResponses);
        }

        public IMockResponseFactory RegisterMockResponse()
        {
            return new MockResponseFactory(this, _serializer);
        }

        public void RegisterMockResponse(IMockResponseDescriptor descriptor)
        {
            _mockResponses.Add(descriptor);
        }
    }
}