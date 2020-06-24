using SereneApi.Factories;
using SereneApi.Interfaces;
using System.Net;

namespace SereneApi.Helpers
{
    /// <summary>
    /// Contains the default values to be used by the <see cref="ApiHandler"/>
    /// </summary>
    public static class ApiHandlerOptionDefaults
    {
        /// <summary>
        /// The default <see cref="IQueryFactory"/> that will ne used by the <see cref="ApiHandler"/>
        /// </summary>
        public static IQueryFactory QueryFactory { get; } = new QueryFactory();

        /// <summary>
        /// The default Timeout Period that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public static int Timeout { get; } = 30;

        /// <summary>
        /// The default Resource Precursor that is used by the <see cref="ApiHandler"/>
        /// </summary>
        public const string ResourcePath = "api/";

        public static ICredentials Credentials { get; } = CredentialCache.DefaultCredentials;

        /// <summary>
        /// The Default retry count used by the <see cref="ApiHandler"/>
        /// </summary>
        public const int RetryCount = 0;
    }
}
