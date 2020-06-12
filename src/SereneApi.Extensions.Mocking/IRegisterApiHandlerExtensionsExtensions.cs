using SereneApi.Extensions.Mocking.Types;
using SereneApi.Helpers;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace SereneApi.Extensions.Mocking
{
    public static class IRegisterApiHandlerExtensionsExtensions
    {
        public static IRegisterApiHandlerExtensions WithMockResponse(this IRegisterApiHandlerExtensions extensions, HttpResponseMessage response)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            HttpMessageHandler mockHttpMessage = new MockHttpMessageHandler(response);

            coreOptions.DependencyCollection.AddDependency(mockHttpMessage);

            return extensions;
        }

        public static IRegisterApiHandlerExtensions WithMockResponse(this IRegisterApiHandlerExtensions extensions, Action<HttpResponseMessage> responseAction)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            responseAction.Invoke(response);

            return WithMockResponse(extensions, response);
        }

        public static IRegisterApiHandlerExtensions WithMockResponse<TContent>(this IRegisterApiHandlerExtensions extensions, TContent content, JsonSerializerOptions serializerOptionsOverride = null)
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

            return WithMockResponse(extensions, response);
        }

        public static IRegisterApiHandlerExtensions WithTimeoutResponse(this IRegisterApiHandlerExtensions extensions)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            HttpMessageHandler mockHttpMessage = new MockHttpMessageHandler();

            coreOptions.DependencyCollection.AddDependency(mockHttpMessage);

            return extensions;
        }

        public static IRegisterApiHandlerExtensions WithTimeoutResponse(this IRegisterApiHandlerExtensions extensions, TimeSpan timeout)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            MockHttpMessageHandler mockHttpMessage = new MockHttpMessageHandler();

            mockHttpMessage.AddWaitUntilSuccess(timeout);

            coreOptions.DependencyCollection.AddDependency((HttpMessageHandler)mockHttpMessage);

            return extensions;
        }

        public static IRegisterApiHandlerExtensions WithTimeout(this IRegisterApiHandlerExtensions extensions, int timeoutCount)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            if (!coreOptions.DependencyCollection.TryGetDependency(out HttpMessageHandler handler))
            {
                throw new MethodAccessException("This method can only be called after AddMockRequest");
            }

            ApiHandlerOptionsRules.ValidateRetryCount(timeoutCount);

            ((MockHttpMessageHandler)handler).AddTimeoutUntilSuccess(timeoutCount);

            coreOptions.DependencyCollection.AddDependency(handler);

            return extensions;
        }

        public static IRegisterApiHandlerExtensions WithTimeout(this IRegisterApiHandlerExtensions extensions, TimeSpan waitTime)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            if (!coreOptions.DependencyCollection.TryGetDependency(out HttpMessageHandler handler))
            {
                throw new MethodAccessException("This method can only be called after AddMockRequest");
            }

            ((MockHttpMessageHandler)handler).AddWaitUntilSuccess(waitTime);

            coreOptions.DependencyCollection.AddDependency(handler);

            return extensions;
        }

        public static IRegisterApiHandlerExtensions WithTimeout(this IRegisterApiHandlerExtensions extensions, int timeoutCount, TimeSpan waitTime)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            if (!coreOptions.DependencyCollection.TryGetDependency(out HttpMessageHandler handler))
            {
                throw new MethodAccessException("This method can only be called after AddMockRequest");
            }

            ApiHandlerOptionsRules.ValidateRetryCount(timeoutCount);

            ((MockHttpMessageHandler)handler).AddTimeoutUntilSuccess(timeoutCount);
            ((MockHttpMessageHandler)handler).AddWaitUntilSuccess(waitTime);

            coreOptions.DependencyCollection.AddDependency(handler);

            return extensions;
        }

        public static IRegisterApiHandlerExtensions HasRequestContent(this IRegisterApiHandlerExtensions extensions, string expectedContent)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            if (!coreOptions.DependencyCollection.TryGetDependency(out HttpMessageHandler handler))
            {
                throw new MethodAccessException("This method can only be called after AddMockRequest");
            }

            ((MockHttpMessageHandler)handler).CheckContent(expectedContent);

            coreOptions.DependencyCollection.AddDependency(handler);

            return extensions;
        }

        public static IRegisterApiHandlerExtensions HasRequestUri(this IRegisterApiHandlerExtensions extensions, string expectedUri)
        {
            CoreOptions coreOptions = GetCoreOptions(extensions);

            if (!coreOptions.DependencyCollection.TryGetDependency(out HttpMessageHandler handler))
            {
                throw new MethodAccessException("This method can only be called after AddMockRequest");
            }

            ((MockHttpMessageHandler)handler).CheckRequestUri(new Uri(expectedUri));

            coreOptions.DependencyCollection.AddDependency(handler);

            return extensions;
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
