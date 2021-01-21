using SereneApi.Abstractions.Events;
using SereneApi.Abstractions.Handler;
using System;

namespace SereneApi.Abstractions.Response.Events
{
    public class ResponseEvent: IEventListener<IApiHandler, IApiResponse>
    {
        public DateTime EventTime { get; }

        public IApiHandler Reference { get; }

        public IApiResponse Value { get; }

        public ResponseEvent(IApiHandler reference, IApiResponse value)
        {
            Reference = reference;
            Value = value;

            EventTime = DateTime.Now;
        }
    }
}
