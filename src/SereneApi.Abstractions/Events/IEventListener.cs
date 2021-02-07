using System;

namespace SereneApi.Abstractions.Events
{
    public interface IEventListener
    {
        DateTimeOffset EventTime { get; }
    }
}
