using System;

namespace SereneApi.Helpers
{
    internal class ApiHandlerOptionsRules
    {
        private const uint _minimumRetryCount = 1;

        public static void ValidateRetryCount(uint retryCount)
        {
            if (retryCount < _minimumRetryCount)
            {
                throw new ArgumentException($"To Enable Retry on Timeout the RetryCount must be greater than {_minimumRetryCount - 1}");
            }
        }

        /// <summary>
        /// If the resource path is null or whitespace the default value will be used.
        /// If the string contains anything other than whitespace the value provided will be used.
        /// Setting an Empty string will override the default
        /// </summary>
        public static string GetResourcePath(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                return ApiHandlerOptionDefaults.ResourcePrecursor;
            }

            return resourcePath;
        }
    }
}
