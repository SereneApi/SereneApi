using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace SereneApi.Testing
{
    public static class ApiHandlerFactoryExtensionsExtension
    {
        public static IApiHandlerFactoryExtensions WithMoqResponse(this IApiHandlerFactoryExtensions extensions, HttpResponseMessage response, Uri requestUri = null)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            HttpMessageHandler mockHttpMessage = new MockHttpMessageHandler(response, requestUri);

            coreOptions.DependencyCollection.AddDependency(mockHttpMessage);

            return extensions;
        }

        public static IApiHandlerFactoryExtensions WithMoqResponse(this IApiHandlerFactoryExtensions extensions, Action<HttpResponseMessage> responseAction, Uri requestUri = null)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            responseAction.Invoke(response);

            return WithMoqResponse(extensions, response, requestUri);
        }

        public static IApiHandlerFactoryExtensions WithMoqResponse<TContent>(this IApiHandlerFactoryExtensions extensions, TContent content, Uri requestUri = null, JsonSerializerOptions serializerOptionsOverride = null)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            JsonSerializerOptions serializerOptions;

            if (serializerOptionsOverride != null)
            {
                serializerOptions = serializerOptionsOverride;
            }
            else
            {
                if (!coreOptions.DependencyCollection.TryGetDependency(out serializerOptions))
                {
                    serializerOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                }
            }

            string stringContent = JsonSerializer.Serialize(content, serializerOptions);

            HttpResponseMessage response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"{stringContent}")
            };

            return WithMoqResponse(extensions, response, requestUri);
        }

        public static IApiHandlerFactoryExtensions WithTimeoutResponse(this IApiHandlerFactoryExtensions extensions)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            HttpMessageHandler mockHttpMessage = new TimeoutMessageHandler();

            coreOptions.DependencyCollection.AddDependency(mockHttpMessage);

            return extensions;
        }

        private static CoreOptions GetCoreOptions(IApiHandlerFactoryExtensions extensions)
        {
            if (extensions is CoreOptions coreOptions)
            {
                return coreOptions;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(CoreOptions)}");
        }
    }
}
