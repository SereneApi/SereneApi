using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SereneApi.Abstractions.Events
{
    public class EventManager: IEventManager
    {
        private readonly Dictionary<Type, List<object>> _events = new Dictionary<Type, List<object>>();

        public void Publish<TEvent>(TEvent sender) where TEvent : IEventListener
        {
            Type eventType = typeof(TEvent);

            if(!_events.TryGetValue(eventType, out List<object> listeners) || listeners.Count <= 0)
            {
                return;
            }

            foreach(Action<TEvent> listener in listeners.Cast<Action<TEvent>>())
            {
                listener.Invoke(sender);
            }
        }

        public Task PublishAsync<TEvent>(TEvent sender) where TEvent : IEventListener
        {
            return Task.Factory.StartNew(() => Publish(sender));
        }

        public void Subscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener
        {
            Type eventType = typeof(TEvent);

            if(_events.TryGetValue(eventType, out List<object> listeners))
            {
                if(!listeners.Contains(listener))
                {
                    listeners.Add(listener);
                }
            }
            else
            {
                listeners = new List<object> { listener };

                _events.Add(eventType, listeners);
            }
        }

        public void UnSubscribe<TEvent>(Action<TEvent> listener) where TEvent : IEventListener
        {
            Type eventType = typeof(TEvent);

            if(!_events.TryGetValue(eventType, out List<object> listeners))
            {
                return;
            }

            if(listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }
    }
}
