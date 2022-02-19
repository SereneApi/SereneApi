using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SereneApi.Core.Events
{
    public class EventManager : IEventManager
    {
        private readonly Dictionary<Type, List<object>> _events = new();

        public void Publish<TEvent>(TEvent sender) where TEvent : IEventListener
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (!_events.TryGetValue(typeof(TEvent), out List<object> listeners) || listeners.Count <= 0)
            {
                return;
            }

            foreach (Action<TEvent> listener in listeners.Cast<Action<TEvent>>())
            {
                listener.Invoke(sender);
            }
        }

        public Task PublishAsync<TEvent>(TEvent sender) where TEvent : IEventListener
        {
            Publish(sender);

            return Task.CompletedTask;
        }

        public void Subscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            if (_events.TryGetValue(typeof(TEvent), out List<object> listeners))
            {
                if (!listeners.Contains(listener))
                {
                    listeners.Add(listener);
                }
            }
            else
            {
                listeners = new List<object> { listener };

                _events.Add(typeof(TEvent), listeners);
            }
        }

        public void UnSubscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            if (!_events.TryGetValue(typeof(TEvent), out List<object> listeners))
            {
                return;
            }

            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}