using SereneApi.Core.Connection;

namespace SereneApi.Core.Handler
{
    public interface IApiHandler
    {
        IConnectionSettings Connection { get; }
    }
}
