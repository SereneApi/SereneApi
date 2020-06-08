using System;

// Do not change namespace
// ReSharper disable once CheckNamespace
namespace SereneApi
{
    public interface IApiResponse
    {
        bool WasSuccessful { get; }

        bool HasException { get; }

        string Message { get; }

        Exception Exception { get; }
    }
}
