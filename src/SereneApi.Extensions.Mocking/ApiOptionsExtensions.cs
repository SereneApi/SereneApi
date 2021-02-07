using DeltaWare.Dependencies.Abstractions;
using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Options;
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
        public static IApiOptionsExtensions WithMockResponse(this IApiOptionsExtensions extensions, [NotNull] Action<IMockResponsesBuilder> mockResponseBuilder, bool enableOutgoingRequests = false)
        {
            if(mockResponseBuilder == null)
            {
                throw new ArgumentNullException(nameof(mockResponseBuilder));
            }

            if(!(extensions is ICoreOptions options))
            {
                throw new InvalidCastException($"Base type must inherit {nameof(ICoreOptions)}");
            }

            options.Dependencies.AddScoped<HttpMessageHandler>(p =>
            {
                MockResponsesBuilder mockResponsesBuilder = new MockResponsesBuilder(p);

                mockResponseBuilder.Invoke(mockResponsesBuilder);

                List<IMockResponse> mockResponses = mockResponsesBuilder.Build();

                if(enableOutgoingRequests)
                {
                    HttpMessageHandler messageHandler = p.GetDependency<IClientFactory>().BuildHttpMessageHandler();

                    return new MockMessageHandler(mockResponses, messageHandler);
                }

                return new MockMessageHandler(mockResponses);
            });

            return extensions;
        }
    }
}
