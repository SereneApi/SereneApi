using SereneApi.Abstractions.Request;
using System;

namespace SereneApi.Helpers
{
    internal static class ExceptionHelper
    {
        public static void EnsureParameterIsNotNull(object parameter, string paramName)
        {
            if(parameter == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
