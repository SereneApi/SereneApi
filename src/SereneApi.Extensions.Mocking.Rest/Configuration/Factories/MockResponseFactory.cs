using SereneApi.Core.Http;
using SereneApi.Core.Http.Content;
using SereneApi.Core.Http.Responses;
using SereneApi.Core.Serialization;
using SereneApi.Extensions.Mocking.Rest.Configuration.Descriptors;
using SereneApi.Extensions.Mocking.Rest.Extensions;
using SereneApi.Extensions.Mocking.Rest.Responses;
using System;
using System.Linq;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Factories
{
    internal class MockResponseFactory : IMockResponseFactory
    {
        private readonly MockingConfiguration _configuration;

        private readonly MockResponseDescriptor _responseDescriptor = new();
        private readonly ISerializer _serializer;
        private readonly IConnectionSettings _connection;

        public MockResponseFactory(MockingConfiguration configuration, ISerializer serializer, IConnectionSettings connection)
        {
            _configuration = configuration;

            _serializer = serializer;
            _connection = connection;
        }

        public IMockResponseData ForContent<TContent>(TContent content)
        {
            IRequestContent serializedContent = _serializer.Serialize(content);

            _responseDescriptor.Content = serializedContent;

            return this;
        }

        public IMockResponseContent ForEndpoints(params Uri[] endpointUris)
        {
            string[] endpoints = new string[endpointUris.Length];

            for (int i = 0; i < endpointUris.Length; i++)
            {
                endpoints[i] = endpointUris[i].ToString();
            }

            return ForEndpoints(endpoints);
        }

        public IMockResponseContent ForEndpoints(params string[] endpoints)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            if (endpoints.Length == 0)
            {
                throw new ArgumentException(null, nameof(endpoints));
            }

            string baseRoute = _connection.GetBaseRoute();

            for (int i = 0; i < endpoints.Length; i++)
            {
                if (endpoints[i].StartsWith(baseRoute))
                {
                    endpoints[i] = endpoints[i].Remove(0, baseRoute.Length);

                    if (endpoints[i].StartsWith('/'))
                    {
                        endpoints[i] = endpoints[i].Substring(1, endpoints[i].Length - 1);
                    }
                }
            }

            endpoints = endpoints.Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

            if (endpoints.Length != 0)
            {
                _responseDescriptor.Endpoints = endpoints;
            }

            return this;
        }

        public IMockResponseEndpoint ForMethod(params HttpMethod[] methods)
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