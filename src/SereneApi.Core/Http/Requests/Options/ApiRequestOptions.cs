using System;

namespace SereneApi.Core.Http.Requests.Options
{
    public class ApiRequestOptions : IApiRequestOptions, IApiRequestOptionsBuilder
    {
        public static ApiRequestOptions Default => new();
        public Action<Exception> OnException { get; set; }
        public Action<TimeoutException> OnTimeout { get; set; }
        public bool ThrowExceptions { get; set; }

        public bool ThrowOnFail { get; set; }

        void IApiRequestOptionsBuilder.ThrowExceptions()
        {
            ThrowExceptions = true;
        }

        void IApiRequestOptionsBuilder.ThrowOnFail()
        {
            ThrowOnFail = true;
        }
    }
}