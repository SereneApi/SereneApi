using System;
using System.Linq;

namespace SereneApi.Helpers
{
    public static class ApiHandlerOptionsHelper
    {
        /// <summary>
        /// Formats the Source for the <see cref="ApiHandler"/> to use when making requests
        /// </summary>
        /// <param name="source">The source of the Api http://someservice.com:8080</param>
        /// <param name="resource">The API Resource for Requests to be made to</param>
        /// <param name="resourcePath">The Path to the Api Resource, by default this is set to "api/"</param>
        /// <returns></returns>
        public static Uri FormatSource(string source, string resource, string resourcePath = null)
        {
            resourcePath = ApiHandlerOptionsRules.GetResourcePath(resourcePath);

            string formattedSource = string.Format(ApiHandlerOptionDefaults.SourceFormat, source, resourcePath, resource);

            return new Uri(formattedSource);
        }

        /// <summary>
        /// Gets the last path from the <see cref="Uri"/>
        /// </summary>
        public static string GetResource(Uri source)
        {
            return source.ToString().Split('/').Last();
        }
    }
}
