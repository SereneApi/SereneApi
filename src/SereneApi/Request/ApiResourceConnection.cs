namespace SereneApi.Request
{
    public readonly struct ApiResourceConnection
    {
        public string HostUrl { get; }

        public string UrlTemplate { get; }

        public ApiResourceConnection(string hostUrl, string urlTemplate)
        {
            HostUrl = hostUrl;
            UrlTemplate = urlTemplate;
        }
    }
}
