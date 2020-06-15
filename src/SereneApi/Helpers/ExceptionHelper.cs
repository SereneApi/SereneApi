using System;

namespace SereneApi.Helpers
{
    internal static class ExceptionHelper
    {
        public static void MethodCannotBeCalledTwice()
        {
            throw new MethodAccessException("This method cannot be called twice.");
        }

        public static void EnsureParameterIsNotNull(object parameter, string paramName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void EnsureArrayIsNotEmpty(object[] array, string paramName)
        {
            if (array.Length == 0)
            {
                throw new ArgumentException($"{paramName} must not be Empty.");
            }
        }
    }
}
