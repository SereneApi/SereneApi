using SereneApi.Abstraction.Enums;
using SereneApi.Extensions.Mocking.Helpers;
using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Interfaces;
using SereneApi.Serializers;
using System.Collections.Generic;
using SereneApi.Interfaces.Requests;

namespace SereneApi.Extensions.Mocking.Types
{
    /// <inheritdoc cref="IMockResponsesBuilder"/>
    public class MockResponsesBuilder: IMockResponsesBuilder
    {
        private ISerializer _serializer = new JsonSerializer();

        private readonly List<IMockResponse> _mockResponses = new List<IMockResponse>();

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
            MockResponse mockResponse = new MockResponse(status, message, null, _serializer);

            _mockResponses.Add(mockResponse);

            return mockResponse.GetExtensions();
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseExtensions AddMockResponse<TContent>(TContent content)
        {
            IApiRequestContent requestContent = _serializer.Serialize(content);

            MockResponse mockResponse = new MockResponse(Status.Ok, null, requestContent, _serializer);

            _mockResponses.Add(mockResponse);

            return mockResponse.GetExtensions();
        }

        /// <inheritdoc>
        ///     <cref>IMockResponsesBuilder.AddMockResponse</cref>
        /// </inheritdoc>
        public IMockResponseExtensions AddMockResponse<TContent>(TContent content, Status status)
        {
            IApiRequestContent requestContent = _serializer.Serialize(content);

            MockResponse mockResponse = new MockResponse(status, null, requestContent, _serializer);

            _mockResponses.Add(mockResponse);

            return mockResponse.GetExtensions();
        }

        /// <summary>
        /// Builds the <see cref="IMockResponse"/>s.
        /// </summary>
        public List<IMockResponse> Build()
        {
            return _mockResponses;
        }
    }
}
