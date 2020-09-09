using SereneApi.Abstractions.Events;
using System;

namespace SereneApi.Abstractions.Response.Events
{
    public class ResponseEvent: IEventListener<IApiResponse>
    {
        public DateTime EventTime { get; }
        public IApiResponse Value { get; }

        public ResponseEvent(IApiResponse response)
        {
            Value = response;

            EventTime = DateTime.Now;
        }
    }
}
