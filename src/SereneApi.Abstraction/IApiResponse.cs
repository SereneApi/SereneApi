using System;

namespace SereneApi.Abstraction
{
    public interface IApiResponse
    {
        bool WasSuccessful { get; }

        bool HasException { get; }

        string Message { get; }

        Exception Exception { get; }
    }
}
