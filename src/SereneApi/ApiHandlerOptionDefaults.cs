using SereneApi.Factories;
using SereneApi.Interfaces;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SereneApi
{
    /// <summary>
    /// Contains the default values to be used by the <see cref="ApiHandler"/>
    /// </summary>
    internal static class ApiHandlerOptionDefaults
    {
        /// <summary>
        /// The default <see cref="IQueryFactory"/> that will ne used by the <see cref="ApiHandler"/>
        /// </summary>
        public static IQueryFactory QueryFactory { get; } = new QueryFactory();

        /// <summary>
        /// The default Timeout Period that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public static TimeSpan TimeoutPeriod { get; } = new TimeSpan(0, 0, 30);

        /// <summary>
        /// The default Resource Precursor that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public const string ResourcePath = "api/";

        /// <summary>
        /// The default <see cref="HttpContentHeaders"/> that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public static Action<HttpRequestHeaders> RequestHeadersBuilder { get; } = headers =>
        {
            headers.Accept.Clear();
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        };

        public static ICredentials Credentials { get; } = CredentialCache.DefaultCredentials;

        /// <summary>
        /// The Default retry count used by the <see cref="ApiHandler"/>
        /// </summary>
        public const uint RetryCount = 0;

        /// <summary>
        /// The Default <see cref="JsonSerializerOptions"/> used by the <see cref="ApiHandler"/>.
        /// </summary>
        public static JsonSerializerOptions JsonSerializerOptionsBuilder = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
