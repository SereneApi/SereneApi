using SereneApi.Abstractions.Factories;
using SereneApi.Abstractions.Requests.Content;
using SereneApi.Abstractions.Serializers;
using System.Net;

namespace SereneApi.Abstractions.Helpers
{
    public static class Defaults
    {
        public static class Factories
        {
            public static IQueryFactory QueryFactory { get; set; } = new DefaultQueryFactory();
        }

        public static class Handler
        {
            public static int Timeout { get; set; } = 30;

            public static string ResourcePath { get; set; } = "api/";

            public static ICredentials Credentials { get; set; } = CredentialCache.DefaultCredentials;

            public static int RetryCount { get; set; } = 0;
        }

        public static uint MinimumRetryCount { get; set; } = 1;

        public static uint MaximumRetryCount { get; set; } = 5;

        public static ISerializer Serializer { get; set; } = new DefaultSerializer();

        public static ContentType ContentType { get; set; } = ContentType.Json;
    }
}
