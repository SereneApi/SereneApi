namespace SereneApi.Core.Configuration
{
    internal interface IHandlerConfiguration
    {
        bool Contains(string key);

        T Get<T>(string key);

        bool TryGet<T>(string key, out T configuration);
    }
}