using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Events
{
    public interface IEventRelay
    {
        void Subscribe<TEvent>([NotNull] Action<TEvent> listener) where TEvent : IEventListener;

        void UnSubscribe<TEvent>([NotNull] Action<TEvent> listener) where TEvent : IEventListener;
    }
}
