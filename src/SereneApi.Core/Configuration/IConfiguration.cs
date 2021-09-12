namespace SereneApi.Core.Configuration
{
    public interface IConfiguration
    {
        string this[string key] { get; }

        bool Contains(string key);

        T Get<T>(string key);
    }
}