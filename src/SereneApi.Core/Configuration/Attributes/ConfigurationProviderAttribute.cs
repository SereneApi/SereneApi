using System;

namespace SereneApi.Core.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationProviderAttribute : Attribute
    {
        public string ProviderName { get; }

        public ConfigurationProviderAttribute(Type provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (!provider.IsSubclassOf(typeof(ConfigurationProvider)))
            {
                throw new ArgumentException($"{provider.Name} must implement {nameof(ConfigurationProvider)}");
            }

            ProviderName = provider.Name;
        }
    }
}
