using System;

namespace SereneApi.Core.Transformation.Exceptions
{
    public class PropertyRequiredException : Exception
    {
        public PropertyRequiredException(string propertyName) : base($"{propertyName} must be not be null", new ArgumentNullException(propertyName))
        {
        }
    }
}
