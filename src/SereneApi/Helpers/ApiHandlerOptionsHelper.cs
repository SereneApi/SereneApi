using System;

namespace DeltaWare.SereneApi.Helpers
{
    public static class ApiHandlerOptionsHelper
    {
        /// <summary>
        /// Creates the Source to be used by a <see cref="ApiHandler"/>
        /// </summary>
        /// <param name="source">The source of the Api http://someservice.com:8080</param>
        /// <param name="resource">The API Resource for Requests to be made to</param>
        /// <param name="resourcePath">The Path to the Api Resource, by default this is set to "api/"</param>
        /// <returns></returns>
        public static Uri CreateApiSource(string source, string resource, string resourcePath = null)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
            {
                resourcePath = ApiHandlerOptionDefaults.ResourcePrecursor;
            }

            return new Uri($"{source}/{resourcePath}{resource}");
        }
    }
}
