using SereneApi.Core.Handler;

namespace SereneApi.Core.Options.Factory
{
    public interface IApiOptionsBuilder<TApiHandler> where TApiHandler : IApiHandler
    {
        IApiOptions<TApiHandler> BuildOptions();
    }
}
