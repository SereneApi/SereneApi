using System;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Settings
{
    internal class DelaySettings : IDelaySettings
    {
        public int Repeats { get; }

        public TimeSpan Time { get; }

        public DelaySettings(TimeSpan time, int repeats = 0)
        {
            Time = time;
            Repeats = repeats;
        }
    }
}