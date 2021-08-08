using System;

namespace SereneApi.Core.Events
{
    public interface IEventRelay
    {
        void Subscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener;

        void UnSubscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener;
    }
}
