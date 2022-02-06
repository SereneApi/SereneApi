using SereneApi.Core.Handler;
using SereneApi.Core.Http.Responses;
using System;

namespace SereneApi.Core.Events.Types
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