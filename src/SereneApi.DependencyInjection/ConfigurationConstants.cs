namespace SereneApi.DependencyInjection
{
    internal class ConfigurationConstants
    {
        public const string ApiConfigKey = "ApiConfig";

        public const string SourceKey = "Source";
        public const string ResourceKey = "Resource";
        public const string ResourcePathKey = "ResourcePath";
        public const string TimeoutKey = "Timeout";
        public const string RetryCountKey = "RetryCount";

        public const bool SourceIsRequired = true;
        public const bool ResourceIsRequired = true;
        public const bool ResourcePathIsRequired = false;
        public const bool TimeoutIsRequired = false;
        public const bool RetryIsRequired = false;

    }
}
