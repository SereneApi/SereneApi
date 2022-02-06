using SereneApi.Core.Handler;
using SereneApi.Core.Http.Requests;
using System;

namespace SereneApi.Core.Events.Types
{
    public class RetryEvent : IEventListener<IApiHandler, IApiRequest>
    {
        public DateTimeOffset EventTime { get; } = DateTimeOffset.Now;

        public IApiHandler Reference { get; }

        public IApiRequest Value { get; }

        public RetryEvent(IApiHandler reference, IApiRequest request)
        {
            Reference = reference;
            Value = request;
        }
    }
}