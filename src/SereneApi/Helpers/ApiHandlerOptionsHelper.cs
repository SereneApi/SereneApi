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
        /// Formats the Source for the <see cref="ApiHandler"/> to use when making requests
        /// </summary>
        /// <param name="source">The source of the Api http://someservice.com:8080</param>
        /// <param name="resource">The API Resource for Requests to be made to</param>
        /// <param name="resourcePath">The Path to the Api Resource, by default this is set to "api/"</param>
        /// <returns></returns>
        public static Uri FormatSource([NotNull] string source, string resource, string resourcePath = null)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Source can not be empty");
            }

            resource = SourceHelpers.EnsureSourceNoSlashTermination(resource);

            resourcePath = UseOrGetDefaultResourcePath(resourcePath);

            string formattedSource = string.Format(ApiHandlerOptionDefaults.SourceFormat, source, resourcePath, resource);

            formattedSource = SourceHelpers.EnsureSourceSlashTermination(formattedSource);

            return new Uri(formattedSource);
        }

        /// <summary>
        /// If the resource path is null or whitespace the default value will be used.
        /// If the string contains anything other than whitespace the value provided will be used.
        /// Setting an Empty string will override the default
        /// </summary>
        public static string UseOrGetDefaultResourcePath(string resourcePath)
        {
            // If an empty string is supplied, it disabled the default value.
            if (resourcePath == string.Empty)
            {
                return string.Empty;
            }

            // Null or whitespace strings will enabled the default.
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                return ApiHandlerOptionDefaults.ResourcePath;
            }

            // If a string is supplied that contains characters it will be used.

            resourcePath = SourceHelpers.EnsureSourceSlashTermination(resourcePath);

            return resourcePath;
        }
    }
}
