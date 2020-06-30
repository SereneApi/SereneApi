using System;

namespace SereneApi.Extensions.DependencyInjection.Helpers
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
