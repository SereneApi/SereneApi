using System;

namespace SereneApi.Core.Configuration.Exceptions
{
    public class ProviderNotInstantiatedException : Exception
    {
        public ProviderNotInstantiatedException(Type type) : base($"{type.Name} could not be instantiated")
        {
        }
    }
}