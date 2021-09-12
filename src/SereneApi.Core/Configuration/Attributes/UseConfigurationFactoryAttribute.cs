using System;

namespace SereneApi.Core.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UseConfigurationFactoryAttribute : Attribute
    {
        public string Name { get; }

        public UseConfigurationFactoryAttribute(Type factoryType)
        {
            if (factoryType == null)
            {
                throw new ArgumentNullException(nameof(factoryType));
            }

            if (!factoryType.IsSubclassOf(typeof(ConfigurationFactory)))
            {
                throw new ArgumentException($"{factoryType.Name} must implement {nameof(ConfigurationFactory)}");
            }

            Name = factoryType.FullName;
        }
    }
}