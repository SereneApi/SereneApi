using SereneApi.Core.Handler;

namespace SereneApi.Core.Options
{
    public interface IApiOptions<TApiHandler> : IApiOptions where TApiHandler : IApiHandler
    {
    }
}