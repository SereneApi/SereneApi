using System;

namespace SereneApi.Extensions.Mocking.Rest.Settings
{
    public class DelaySettings
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