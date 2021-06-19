using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Requests;
using System;

namespace SereneApi.Abstractions.Events.Types
{
    public class RequestEvent : IEventListener<IApiHandler, IApiRequest>
    {
        public DateTimeOffset EventTime { get; } = DateTimeOffset.Now;

        public IApiHandler Reference { get; }

        public IApiRequest Value { get; }

        public RequestEvent(IApiHandler reference, IApiRequest value)
        {

            Reference = reference;
            Value = value;
        }

    }
}
