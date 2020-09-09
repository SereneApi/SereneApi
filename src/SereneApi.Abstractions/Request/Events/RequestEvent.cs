using SereneApi.Abstractions.Events;
using System;

namespace SereneApi.Abstractions.Request.Events
{
    public class RequestEvent: IEventListener<IRequest>
    {
        public IRequest Value { get; }

        public RequestEvent(IRequest value)
        {
            Value = value;

            EventTime = DateTime.Now;
        }

        public DateTime EventTime { get; }
    }
}
