using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Events
{
    public interface IEventManager: IEventRelay
    {
        void Publish<TEvent>([NotNull] TEvent sender) where TEvent : IEventListener;

        Task PublishAsync<TEvent>([NotNull] TEvent sender) where TEvent : IEventListener;
    }
}
