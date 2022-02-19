using System;

namespace SereneApi.Extensions.Mocking.Rest.Responses.Factories
{
    public interface IMockResponseDelay
    {
        void IsDelayed(TimeSpan time, int repeats = 0);

        void IsDelayed(int seconds, int repeats = 0);
    }
}