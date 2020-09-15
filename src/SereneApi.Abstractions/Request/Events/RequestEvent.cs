using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Handler;
using System;

namespace SereneApi.Abstractions.Request.Events
{
    public class RequestEvent: IEventListener<IApiHandler, IApiRequest>
    {
        public IApiHandler Reference { get; }

        public IApiRequest Value { get; }

        public RequestEvent(IApiHandler reference, IApiRequest value)
        {
            Reference = reference;
            Value = value;

            EventTime = DateTime.Now;
        }

        public DateTime EventTime { get; }
    }
}
