using System;

namespace SereneApi.Core.Http.Requests.Options
{
    public interface IApiRequestOptionsBuilder
    {
        Action<Exception> OnException { get; set; }

        Action<TimeoutException> OnTimeout { get; set; }

        void ThrowExceptions();

        void ThrowOnFail();
    }
}