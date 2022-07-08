using SereneApi.Core.Handler;

namespace SereneApi.Handlers.Rest.Requests.Factories
{
    public interface IApiRequestFactory : IApiRequestMethod, IApiRequestResource
    {
        IApiHandler Handler { get; set; }
    }
}