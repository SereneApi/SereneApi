using SereneApi.Extensions.Mocking.Interfaces;
using SereneApi.Extensions.Mocking.Types;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SereneApi.Extensions.Mocking
{
    public static class IRegisterApiHandlerExtensionsExtensions
    {
        public static void WithMockResponses(this IRegisterApiHandlerExtensions registrationExtensions, Action<IMockResponsesBuilder> mockResponseBuilder)
        {
            CoreOptions coreOptions = GetCoreOptions(registrationExtensions);

            MockResponsesBuilder builder = new MockResponsesBuilder();

            mockResponseBuilder.Invoke(builder);

            List<IMockResponse> mockResponses = builder.Build();

            HttpMessageHandler mockHandler = new MockMessageHandler(mockResponses);

            coreOptions.DependencyCollection.AddDependency(mockHandler);
        }

        private static CoreOptions GetCoreOptions(IRegisterApiHandlerExtensions extensions)
        {
            if (extensions is CoreOptions coreOptions)
            {
                return coreOptions;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(CoreOptions)}");
        }
    }
}
