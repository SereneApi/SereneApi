using System;

namespace SereneApi.Core.Http.Requests.Options
{
    public interface IApiRequestOptions
    {
        Action<Exception> OnException { get; }
        Action<TimeoutException> OnTimeout { get; }
        bool ThrowExceptions { get; }
        bool ThrowOnFail { get; }
    }
}