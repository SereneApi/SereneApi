using System;

namespace SereneApi.Abstractions
{
    public interface IApiResponse
    {
        bool WasSuccessful { get; }

        bool HasException { get; }

        string Message { get; }

        Exception Exception { get; }
    }
}
