using SereneApi.Abstractions.Request;
using System;

namespace SereneApi.Abstractions.Helpers
{
    internal static class ExceptionHelper
    {
        /// <summary>
        /// Throws a <see cref="TimeoutException"/>.
        /// </summary>
        public static void RequestTimedOut(Uri route, int requestsAttempted)
        {
            throw new TimeoutException($"The Request to \"{route}\" has Timed Out; Retry limit reached. Retired {requestsAttempted}");
        }

        public static void MethodCannotBeCalledTwice()
        {
            throw new MethodAccessException("This method cannot be called twice.");
        }

        public static void EnsureParameterIsNotNull(object parameter, string paramName)
        {
            if(parameter == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void EnsureArrayIsNotEmpty(object[] array, string paramName)
        {
            if(array.Length == 0)
            {
                throw new ArgumentException($"{paramName} must not be Empty.");
            }
        }

        public static void EnsureCorrectMethod(Method method)
        {
            if(method == Method.NONE)
            {
                throw new ArgumentException("An invalid method was provided.");
            }
        }
    }
}
