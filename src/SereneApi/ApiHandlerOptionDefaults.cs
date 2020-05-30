﻿using System;
using System.Net;
using System.Net.Http.Headers;
using SereneApi.Factories;
using SereneApi.Interfaces;

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
        public const string ResourcePrecursor = "api/";

        /// <summary>
        /// The default <see cref="HttpContentHeaders"/> that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public static Action<HttpRequestHeaders> RequestHeadersBuilder { get; } = headers =>
        {
            headers.Accept.Clear();
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        };

        public static ICredentials Credentials { get; } = CredentialCache.DefaultCredentials;

        public const uint RetryCount = 0;

        /// <summary>
        /// The Source format string, used to create the Api Source.
        /// {0 = Source}; {1 = Resource Path}; {2 = Resource}
        /// </summary>
        public const string SourceFormat = "{0}/{1}{2}";
    }
}