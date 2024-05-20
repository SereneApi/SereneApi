namespace SereneApi.Request
{
    internal interface IApiResourceConnection
    {
        string HostUrl { get; }

        string UrlTemplate { get; }
    }
}
