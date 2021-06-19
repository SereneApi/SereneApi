using SereneApi.Abstractions.Events;

namespace SereneApi.Abstractions.Configuration.Adapters
{
    public interface IApiAdapter
    {
        IEventRelay Events { get; }
    }
}
