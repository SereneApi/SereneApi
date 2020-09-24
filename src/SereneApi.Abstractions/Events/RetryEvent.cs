using SereneApi.Abstractions.Handler;
using System;
using SereneApi.Abstractions.Request;

namespace SereneApi.Abstractions.Events
{
    public class RetryEvent: IEventListener<IApiHandler, Guid>
    {
        public DateTime EventTime { get; }

        public IApiHandler Reference { get; }

        public Guid Value { get; }

        public RetryEvent(IApiHandler reference, IApiRequest request)
        {
            Reference = reference;
            Value = request.Identity;

            EventTime = DateTime.Now;
        }
    }
}
