using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Requests;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Responses.Configuration;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Factories
{
    internal class MockResponseFactory : IMockResponseFactory
    {
        private readonly IMockingConfiguration _configuration;

        private readonly MockResponseDescriptor _responseDescriptor = new();
        private readonly ISerializer _serializer;

        public MockResponseFactory(IMockingConfiguration configuration, ISerializer serializer)
        {
            _configuration = configuration;

            _serializer = serializer;
        }

        public IMockResponseData ForContent<TContent>(TContent content)
        {
            IRequestContent serializedContent = _serializer.Serialize(content);

            _responseDescriptor.Content = serializedContent;

            return this;
        }

        public IMockResponseContent ForEndpoints(params string[] endpoints)
        {
            Uri[] endpointUris = new Uri[endpoints.Length];

            for (int i = 0; i < endpoints.Length; i++)
            {
                endpointUris[i] = new Uri(endpoints[i]);
            }

            return ForEndpoints(endpointUris);
        }

        public IMockResponseContent ForEndpoints(params Uri[] endpoints)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            if (endpoints.Length == 0)
            {
                throw new ArgumentException(null, nameof(endpoints));
            }

            _responseDescriptor.Endpoints = endpoints;

            return this;
        }

        public IMockResponseEndpoint ForMethod(params Method[] methods)
        {
            if (methods == null)
            {
                throw new ArgumentNullException(nameof(methods));
            }

            if (methods.Length == 0)
            {
                throw new ArgumentException(null, nameof(methods));
            }

            _responseDescriptor.Methods = methods;

            return this;
        }

        public IMockResponseDelay RespondsWith(Status status)
        {
            MockResponse mockResponse = new MockResponse { Status = status };

            _responseDescriptor.Response = mockResponse;

            _configuration.RegisterMockResponse(_responseDescriptor);

            return new MockResponseDelay(mockResponse);
        }

        public IMockResponseDelay RespondsWith<TContent>(TContent content, Status status = Status.Ok)
        {
            IRequestContent serializedContent = _serializer.Serialize(content);

            MockResponse mockResponse = new MockResponse { Content = serializedContent, Status = status };

            _responseDescriptor.Response = mockResponse;

            _configuration.RegisterMockResponse(_responseDescriptor);

            return new MockResponseDelay(mockResponse);
        }
    }
}