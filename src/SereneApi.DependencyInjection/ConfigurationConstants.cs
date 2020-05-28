namespace SereneApi.DependencyInjection
{
    internal class ConfigurationConstants
    {
        public const string SourceKey = "Source";
        public const string ResourceKey = "Resource";
        public const string ResourcePathKey = "ResourcePrecursor";
        public const string TimeoutKey = "Timeout";

        public const bool SourceIsRequired = true;
        public const bool ResourceIsRequired = true;
        public const bool ResourcePathIsRequired = false;
        public const bool TimeoutIsRequired = false;
    }
}
