using System;

namespace SereneApi.Abstractions.Events
{
    public interface IEventListener
    {
        DateTime EventTime { get; }
    }
}
