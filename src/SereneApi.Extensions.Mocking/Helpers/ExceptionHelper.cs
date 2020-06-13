using System;

namespace SereneApi.Extensions.Mocking.Helpers
{
    internal static class ExceptionHelper
    {
        public static void MethodCannotBeCalledTwice()
        {
            throw new MethodAccessException("This method cannot be called twice.");
        }
    }
}
