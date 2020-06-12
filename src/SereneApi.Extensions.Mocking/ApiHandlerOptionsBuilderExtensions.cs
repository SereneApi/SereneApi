using SereneApi.Extensions.Mocking.Types;
using SereneApi.Interfaces;
using SereneApi.Types;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace SereneApi.Extensions.Mocking
{
    public static class ApiHandlerOptionsBuilderExtensions
    {
        public static void AddMoqResponse(this IApiHandlerOptionsBuilder builder, HttpResponseMessage response)
        {
            HttpMessageHandler mockHttpMessage = new MockHttpMessageHandler(response);

            builder.UseHttpMessageHandler(mockHttpMessage);
        }

        public static void AddMoqResponse(this IApiHandlerOptionsBuilder builder, Action<HttpResponseMessage> responseAction)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            responseAction.Invoke(response);

            AddMoqResponse(builder, response);
        }

        public static void AddMoqResponse<TContent>(this IApiHandlerOptionsBuilder builder, TContent content, JsonSerializerOptions serializerOptionsOverride = null)
        {
            CoreOptions coreOptionsBuilder = GetCoreOptions(builder);

            JsonSerializerOptions serializerOptions;

            if (serializerOptionsOverride != null)
            {
                serializerOptions = serializerOptionsOverride;
            }
            else
            {
                if (!coreOptionsBuilder.DependencyCollection.TryGetDependency(out serializerOptions))
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

            AddMoqResponse(builder, response);
        }

        private static CoreOptions GetCoreOptions(IApiHandlerOptionsBuilder builder)
        {
            if (builder is CoreOptions coreOptions)
            {
                return coreOptions;
            }

            throw new TypeAccessException($"Must be of type or inherit from {nameof(CoreOptions)}");
        }
    }
}
