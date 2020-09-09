using System;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Events
{
    public interface IEventManager
    {
        void Publish<TEvent>(TEvent sender) where TEvent : IEventListener;

        Task PublishAsync<TEvent>(TEvent sender) where TEvent : IEventListener;

        void Subscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener;

        void UnSubscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener;
    }
}
