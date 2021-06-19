using SereneApi.Abstractions.Handler;
using SereneApi.Abstractions.Response;
using System;

namespace SereneApi.Abstractions.Events.Types
{
    public class ResponseEvent : IEventListener<IApiHandler, IApiResponse>
    {
        public DateTimeOffset EventTime { get; } = DateTimeOffset.Now;

        public IApiHandler Reference { get; }

        public IApiResponse Value { get; }

        public ResponseEvent(IApiHandler reference, IApiResponse value)
        {
            Reference = reference;
            Value = value;
        }
    }
}
