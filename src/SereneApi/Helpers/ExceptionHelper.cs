using System;

namespace SereneApi.Helpers
{
    internal static class ExceptionHelper
    {
        public static void MethodCannotBeCalledTwice()
        {
            throw new MethodAccessException("This method cannot be called twice.");
        }
    }
}
