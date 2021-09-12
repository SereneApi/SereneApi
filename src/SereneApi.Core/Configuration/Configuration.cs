using System.Collections.Generic;

namespace SereneApi.Core.Configuration
{
    public class Configuration : IConfiguration
    {
        private readonly IDictionary<string, object> _parameters;

        public string this[string key] => _parameters[key].ToString();

        public Configuration(IDictionary<string, object> parameters = null)
        {
            _parameters = parameters ?? new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            if (!_parameters.TryAdd(key, value))
            {
                _parameters[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return _parameters.ContainsKey(key);
        }

        public T Get<T>(string key)
        {
            return (T)_parameters[key];
        }

        public void Override(string key, object value)
        {
            _parameters[key] = value;
        }
    }
}