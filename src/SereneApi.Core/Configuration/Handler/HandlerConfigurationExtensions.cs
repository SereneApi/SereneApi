using SereneApi.Core.Http.Content;
using System;
using System.Collections.Generic;

namespace SereneApi.Core.Configuration.Handler
{
    public static class HandlerConfigurationExtensions
    {
        public static Type GetHandlerType(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<Type>(HandlerConfigurationKeys.HandlerType);
        }

        public static bool TryGetHandlerType(this HandlerConfiguration handlerConfiguration, out Type handlerType)
        {
            return handlerConfiguration.TryGet(HandlerConfigurationKeys.HandlerType, out handlerType);
        }

        public static void SetHandlerType(this HandlerConfiguration handlerConfiguration, Type handlerType)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.HandlerType, handlerType);
        }

        public static ContentType GetContentType(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<ContentType>(HandlerConfigurationKeys.ContentType);
        }

        public static Dictionary<string, string> GetRequestHeaders(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<Dictionary<string, string>>(HandlerConfigurationKeys.RequestHeaders);
        }

        public static string GetResourcePath(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<string>(HandlerConfigurationKeys.ResourcePath);
        }

        public static int GetRetryAttempts(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<int>(HandlerConfigurationKeys.RetryAttempts);
        }

        public static bool GetThrowExceptions(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<bool>(HandlerConfigurationKeys.ThrowExceptions);
        }

        public static bool GetThrowOnFailure(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<bool>(HandlerConfigurationKeys.ThrowOnFailure);
        }

        public static int GetTimeout(this HandlerConfiguration handlerConfiguration)
        {
            return handlerConfiguration.Get<int>(HandlerConfigurationKeys.Timeout);
        }

        public static void SetContentType(this HandlerConfiguration handlerConfiguration, ContentType type)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.ContentType, type);
        }

        public static void SetRequestHeaders(this HandlerConfiguration handlerConfiguration, Dictionary<string, string> headers)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.RequestHeaders, headers);
        }

        public static void SetResourcePath(this HandlerConfiguration handlerConfiguration, string resourcePath)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.ResourcePath, resourcePath);
        }

        public static void SetRetryAttempts(this HandlerConfiguration handlerConfiguration, int retryAttempts)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.RetryAttempts, retryAttempts);
        }

        public static void SetThrowExceptions(this HandlerConfiguration handlerConfiguration, bool throwExceptions)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.ThrowExceptions, throwExceptions);
        }

        public static void SetThrowOnFailure(this HandlerConfiguration handlerConfiguration, bool throwExceptions)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.ThrowOnFailure, throwExceptions);
        }

        public static void SetTimeout(this HandlerConfiguration handlerConfiguration, int timeoutSeconds)
        {
            handlerConfiguration.Add(HandlerConfigurationKeys.Timeout, timeoutSeconds);
        }

        public static bool TryGetContentType(this HandlerConfiguration handlerConfiguration, out ContentType contentType)
        {
            return handlerConfiguration.TryGet(HandlerConfigurationKeys.ContentType, out contentType);
        }
    }
}