using SereneApi.Core.Configuration.Provider;
using System;

namespace SereneApi.Core.Configuration.Exceptions
{
    public class HandlerNullProviderException : Exception
    {
        public HandlerNullProviderException(Type type) : base($"{type.Name} is not assigned to any {nameof(HandlerConfigurationProvider)} and cannot be instantiated.")
        {
        }
    }
}