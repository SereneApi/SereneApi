using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Extensions.Mocking.Types;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Net.Http;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi.Extensions.Mocking
{
    public static class IRegisterApiHandlerExtensionsExtensions
    {
        /// <summary>
        /// Adds the specified <see cref="IMockResponse"/>s to the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="registrationExtensions">The extensions that the <see cref="IMockResponse"/>s will be appended to.</param>
        /// <param name="mockResponseBuilder">The <see cref="IMockResponse"/>s to be added to the <see cref="ApiHandler"/>.</param>
        /// <param name="enableOutgoingRequests">If set to true, any request that does not have an associated <see cref="IMockResponse"/> will be processed normally.
        /// If set to false, if a request does not have an associated <see cref="IMockResponse"/> an <see cref="ArgumentException"/> will be thrown.</param>
        public static IApiHandlerExtensions WithMockResponses(this IApiHandlerExtensions registrationExtensions, Action<IMockResponsesBuilder> mockResponseBuilder, bool enableOutgoingRequests = false)
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

            MockResponsesBuilder mockResponsesBuilder = new MockResponsesBuilder();

            mockResponseBuilder.Invoke(mockResponsesBuilder);

            HttpMessageHandler mockHandler;

            if(enableOutgoingRequests)
            {
                mockHandler = new MockMessageHandler(new HttpClientHandler(), mockResponsesBuilder);
            }
            else
            {
                mockHandler = new MockMessageHandler(mockResponsesBuilder);
            }

            coreOptions.DependencyCollection.AddDependency(mockHandler);

            return registrationExtensions;
        }

        private static CoreOptions GetCoreOptions(IApiHandlerExtensions extensions)
        {
            if(extensions is CoreOptions coreOptions)
            {
                return coreOptions;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(CoreOptions)}");
        }
    }
}
