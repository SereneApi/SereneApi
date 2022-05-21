using SereneApi.Extensions.Mocking.Rest.Configuration.Settings;
using SereneApi.Extensions.Mocking.Rest.Responses;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Factories
{
    internal class MockResponseDelay : IMockResponseDelay
    {
        private readonly MockResponse _response;

        public MockResponseDelay(MockResponse response)
        {
            _response = response;
        }

        public void AddDelay(int seconds)
        {
        }

        public void IsDelayed(TimeSpan time, int repeats = 0)
        {
            if (time == TimeSpan.Zero)
            {
                throw new ArgumentException("Delay time must be greater than 0 seconds", nameof(time));
            }

            _response.Delay = new DelaySettings(time, repeats);
        }

        public void IsDelayed(int seconds, int repeats)
        {
            if (seconds == 0)
            {
                throw new ArgumentException("Delay time must be greater than 0 seconds", nameof(seconds));
            }

            _response.Delay = new DelaySettings(TimeSpan.FromSeconds(seconds), repeats);
        }
    }
}