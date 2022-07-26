using System;
using System.Collections.Generic;

namespace SereneApi.Core.Configuration.Handler
{
    public sealed class HandlerConfiguration : IHandlerConfiguration
    {
        private readonly IDictionary<string, object> _configurations;

        public HandlerConfiguration(IDictionary<string, object> parameters = null)
        {
            _configurations = parameters ?? new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            if (!_configurations.TryAdd(key, value))
            {
                _configurations[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return _configurations.ContainsKey(key);
        }

        public T Get<T>(string key)
        {
            object configuration = _configurations[key];

            if (configuration is T value)
            {
                return value;
            }

            throw new InvalidCastException($"Attempted to load handlerConfiguration {key} as {typeof(T).Name} but it was added as {configuration.GetType().Name}");
        }

        public bool TryGet<T>(string key, out T configuration)
        {
            configuration = default;

            if (!_configurations.TryGetValue(key, out object configurationObject))
            {
                return false;
            }

            if (configurationObject is not T configurationValue)
            {
                throw new InvalidCastException($"Attempted to load handlerConfiguration {key} as {typeof(T).Name} but it was added as {configuration.GetType().Name}");
            }

            configuration = configurationValue;

            return true;
        }
    }
}