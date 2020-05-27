using DeltaWare.SereneApi.Interfaces;
using System;
using System.Net.Http.Headers;

namespace DeltaWare.SereneApi
{
    /// <summary>
    /// Contains the default values to be used by the <see cref="ApiHandler"/>
    /// </summary>
    public static class ApiHandlerOptionDefaults
    {
        /// <summary>
        /// The default <see cref="IQueryFactory"/> that will ne used by the <see cref="ApiHandler"/>
        /// </summary>
        public static readonly IQueryFactory QueryFactory = new QueryFactory();

        /// <summary>
        /// The default Timeout Period that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public static readonly TimeSpan TimeoutPeriod = new TimeSpan(0, 0, 30);

        /// <summary>
        /// The default Resource Precursor that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public static readonly string ResourcePrecursor = "api/";

        /// <summary>
        /// The default <see cref="HttpContentHeaders"/> that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public static Action<HttpRequestHeaders> DefaultRequestHeadersBuilder { get; } = headers =>
        {
            headers.Accept.Clear();
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        };

        public static uint RetryCount = 0;
    }
}
