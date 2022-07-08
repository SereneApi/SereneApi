using Microsoft.Extensions.Logging;
using SereneApi.Core.Http;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors;
using SereneApi.Extensions.Mocking.Rest.Configuration.Factories;
using SereneApi.Extensions.Mocking.Rest.Handler;
using SereneApi.Extensions.Mocking.Rest.Handler.Attributes;
using SereneApi.Extensions.Mocking.Rest.Handler.Manager;
using SereneApi.Extensions.Mocking.Rest.Responses.Manager;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SereneApi.Extensions.Mocking.Rest.Configuration
{
    internal class MockingConfiguration : IMockingConfiguration
    {
        private readonly List<IMockRequestDescriptor> _mockResponses = new();
        private readonly ISerializer _serializer;
        private readonly IConnectionSettings _connection;
        private readonly IMockHandlerManager _mockHandlerManager;
        private readonly ILogger _logger;

        public MockingConfiguration(ISerializer serializer, IConnectionSettings connection, IMockHandlerManager mockHandlerManager, ILogger logger = null)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mockHandlerManager = mockHandlerManager ?? throw new ArgumentNullException(nameof(mockHandlerManager));

            _logger = logger;
        }

        public IMockResponseManager BuildManager()
        {
            return new MockResponseManager(_mockResponses, _connection, _mockHandlerManager, _logger);
        }

        public IMockResponseFactory RegisterMockResponse()
        {
            return new MockResponseFactory(this, _serializer, _connection);
        }

        public void RegisterMockResponse(IMockRequestDescriptor descriptor)
        {
            _mockResponses.Add(descriptor);
        }

        public void RegisterMockingHandler<T>() where T : MockRestApiHandlerBase
        {
            Type mockingHandlerType = typeof(T);

            MethodInfo[] methods = mockingHandlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                MockMethodAttribute mockMethod = method.GetCustomAttribute<MockMethodAttribute>();

                if (mockMethod == null)
                {
                    continue;
                }

                _mockResponses.Add(new MockHandlerDescriptor
                {
                    HandlerType = mockingHandlerType,
                    Method = method,
                    MockMethod = mockMethod,
                    Methods = new[] { mockMethod.HttpMethod },
                    Endpoints = new[] { mockMethod.EndpointTemplate }
                });
            }
        }
    }
}