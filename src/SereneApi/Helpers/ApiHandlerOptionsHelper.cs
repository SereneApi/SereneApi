using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Helpers
{
    /// <summary>
    /// Contains methods to help with <see cref="IApiHandlerOptions"/>
    /// </summary>
    public static class ApiHandlerOptionsHelper
    {
        /// <summary>
        /// If the resource path is null or whitespace the default value will be used.
        /// If the string contains anything other than whitespace the value provided will be used.
        /// Setting an Empty string will disable the default value.
        /// </summary>
        public static string UseOrGetDefaultResourcePath(string resourcePath)
        {
            // If an empty string is supplied, the default value is disabled.
            if (resourcePath == string.Empty)
            {
                return string.Empty;
            }

            // Null or whitespace strings will enabled the default.
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                resourcePath = ApiHandlerOptionDefaults.ResourcePath;
            }

            resourcePath = SourceHelpers.EnsureSourceSlashTermination(resourcePath);

            return resourcePath;
        }
    }
}
