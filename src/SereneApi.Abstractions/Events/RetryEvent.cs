using SereneApi.Abstractions.Handler;
using System;

namespace SereneApi.Abstractions.Events
{
    public class RetryEvent: IEventListener<IApiHandler, int>
    {
        public DateTime EventTime { get; }

        public IApiHandler Reference { get; }

        public int Value { get; }

        public RetryEvent(IApiHandler reference, int value)
        {
            Reference = reference;
            Value = value;

            EventTime = DateTime.Now;
        }
    }
}
