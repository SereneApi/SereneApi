using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Options;
using SereneApi.Extensions.Mocking.Handlers;
using SereneApi.Extensions.Mocking.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking
{
    public static class RegisterApiHandlerExtensionsExtensions
    {
        /// <summary>
        /// Adds the specified <see cref="IMockResponse"/>s to the <see cref="ApiHandler"/>.
        /// </summary>
        /// <param name="registrationExtensions">The extensions that the <see cref="IMockResponse"/>s will be appended to.</param>
        /// <param name="mockResponseBuilder">The <see cref="IMockResponse"/>s to be added to the <see cref="ApiHandler"/>.</param>
        /// <param name="enableOutgoingRequests">If set to true, any request that does not have an associated <see cref="IMockResponse"/> will be processed normally.
        /// If set to false, if a request does not have an associated <see cref="IMockResponse"/> an <see cref="ArgumentException"/> will be thrown.</param>
        public static IApiOptionsExtensions WithMockResponses(this IApiOptionsExtensions registrationExtensions, Action<IMockResponsesBuilder> mockResponseBuilder, bool enableOutgoingRequests = false)
        {
            IDependencyCollection dependencies = GetDependencies(registrationExtensions);

            MockResponsesBuilder mockResponsesBuilder = new MockResponsesBuilder();

            mockResponseBuilder.Invoke(mockResponsesBuilder);

            dependencies.AddScoped<HttpMessageHandler>(() =>
            {
                List<IMockResponse> mockResponses = mockResponsesBuilder.Build();

                if(enableOutgoingRequests)
                {
                    return new MockMessageHandler(mockResponses, new HttpClientHandler());
                }

                return new MockMessageHandler(mockResponses);
            });

            return registrationExtensions;
        }

        private static IDependencyCollection GetDependencies(IApiOptionsExtensions extensions)
        {
            if(extensions is ICoreOptions options)
            {
                return options.Dependencies;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(ICoreOptions)}");
        }
    }
}
