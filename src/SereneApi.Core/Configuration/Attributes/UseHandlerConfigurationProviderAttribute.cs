using SereneApi.Core.Configuration.Provider;
using System;

namespace SereneApi.Core.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UseHandlerConfigurationProviderAttribute : Attribute
    {
        public string Name => Type.Name;

        public Type Type { get; }

        public UseHandlerConfigurationProviderAttribute(Type factoryType)
        {
            if (factoryType == null)
            {
                throw new ArgumentNullException(nameof(factoryType));
            }

            Type baseType = typeof(HandlerConfigurationProvider);

            if (factoryType != baseType && !factoryType.IsSubclassOf(typeof(HandlerConfigurationProvider)))
            {
                throw new ArgumentException($"{factoryType.Name} must implement {nameof(HandlerConfigurationProvider)}");
            }

            Type = factoryType;
        }
    }
}