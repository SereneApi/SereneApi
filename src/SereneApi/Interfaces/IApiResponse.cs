using System;

namespace SereneApi.Interfaces
{
    public interface IApiResponse
    {
        bool WasSuccessful { get; }

        bool HasException { get; }

        string Message { get; }

        Exception Exception { get; }
    }
}
