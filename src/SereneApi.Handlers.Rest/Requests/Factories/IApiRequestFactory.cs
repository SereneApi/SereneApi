namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestFactory : IApiRequestMethod, IApiRequestResource
    {
        RestApiHandler Handler { get; set; }
    }
}