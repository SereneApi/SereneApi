using System;

namespace SereneApi.Core.Events
{
    public interface IEventListener
    {
        DateTimeOffset EventTime { get; }
    }
}
