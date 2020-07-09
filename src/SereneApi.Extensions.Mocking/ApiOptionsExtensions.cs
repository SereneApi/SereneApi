using DeltaWare.Dependencies;
using SereneApi.Abstractions.Configuration;
using SereneApi.Abstractions.Options;
using SereneApi.Extensions.Mocking.Handlers;
using SereneApi.Extensions.Mocking.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking
{
    public static class ApiOptionsExtensions
    {
        /// <summary>
        /// Adds the specified <see cref="IMockResponse"/>s
        /// </summary>
        /// <param name="mockResponseBuilder">The <see cref="IMockResponse"/>s to be added.</param>
        /// <param name="enableOutgoingRequests">If set to true, any request that does not have an associated <see cref="IMockResponse"/> will be processed normally.
        /// If set to false, if a request does not have an associated <see cref="IMockResponse"/> an <see cref="ArgumentException"/> will be thrown.</param>
        /// <exception cref="ArgumentNullException">Thrown if a null value is provided.</exception>
        public static IApiOptionsExtensions WithMockResponses(this IApiOptionsExtensions registrationExtensions, [NotNull] Action<IMockResponsesBuilder> mockResponseBuilder, bool enableOutgoingRequests = false)
        {
            if(mockResponseBuilder == null)
            {
                throw new ArgumentNullException(nameof(mockResponseBuilder));
            }

            IDependencyCollection dependencies = GetDependencies(registrationExtensions);

            dependencies.AddScoped<HttpMessageHandler>(p =>
            {
                MockResponsesBuilder mockResponsesBuilder = new MockResponsesBuilder(p);

                mockResponseBuilder.Invoke(mockResponsesBuilder);

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
