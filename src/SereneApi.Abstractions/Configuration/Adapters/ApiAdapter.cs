using SereneApi.Abstractions.Events;

namespace SereneApi.Abstractions.Configuration.Adapters
{
    internal class ApiAdapter : IApiAdapter
    {
        public IEventRelay Events { get; }

        public ApiAdapter(IEventRelay eventRelay)
        {
            Events = eventRelay;
        }
    }
}
