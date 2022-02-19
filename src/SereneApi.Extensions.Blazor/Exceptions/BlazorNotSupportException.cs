using System;

namespace SereneApi.Extensions.Blazor.Exceptions
{
    public class BlazorNotSupportException : Exception
    {
        internal BlazorNotSupportException(string name) : base($"{name} is not supported in Blazor.")
        {
        }

        internal BlazorNotSupportException(Type type) : this(type.Name)
        {
        }
    }
}