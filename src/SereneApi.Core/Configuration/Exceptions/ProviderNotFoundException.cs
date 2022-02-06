using System;

namespace SereneApi.Core.Configuration.Exceptions
{
    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(Type type) : base($"{type.Name} is not registered to the {nameof(ApiConfigurationManager)}")
        {
        }
    }
}