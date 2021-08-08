using SereneApi.Core.Handler;

namespace SereneApi.Core.Options.Builder
{
    public interface IApiOptionsFactory<TApiHandler> where TApiHandler : IApiHandler
    {
        IApiOptions<TApiHandler> BuildOptions();
    }
}
