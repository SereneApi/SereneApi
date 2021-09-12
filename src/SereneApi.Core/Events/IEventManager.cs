using System.Threading.Tasks;

namespace SereneApi.Core.Events
{
    public interface IEventManager : IEventRelay
    {
        void Publish<TEvent>(TEvent sender) where TEvent : IEventListener;

        Task PublishAsync<TEvent>(TEvent sender) where TEvent : IEventListener;
    }
}