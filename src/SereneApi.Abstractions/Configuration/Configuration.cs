namespace SereneApi.Abstractions.Configuration
{
    public class Configuration: IConfiguration
    {
        public IApiHandlerConfiguration ApiHandler { get; set; }

        private Configuration()
        {
        }

        public static IConfiguration Default { get; } = new Configuration
        {
            ApiHandler = ApiHandlerConfiguration.Default
        };
    }
}
