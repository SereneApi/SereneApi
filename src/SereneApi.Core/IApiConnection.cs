namespace SereneApi.Core
{
    public interface IApiConnection
    {
        string BaseUrl { get; }

        string Environment { get; }
    }
}