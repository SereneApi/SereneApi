namespace SereneApi.Core.Configuration
{
    public interface IHandlerConfiguration
    {
        string ResourcePath { get; }

        int Timeout { get; }

        int RetryCount { get; }
    }
}
