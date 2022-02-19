using SereneApi.Core.Handler;

namespace SereneApi.Core.Configuration.Settings
{
    public interface IApiSettings<TApiHandler> : IApiSettings where TApiHandler : IApiHandler
    {
    }
}