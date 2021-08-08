using SereneApi.Core.Handler;
using SereneApi.Core.Requests;
using System;

namespace SereneApi.Core.Events.Types
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
