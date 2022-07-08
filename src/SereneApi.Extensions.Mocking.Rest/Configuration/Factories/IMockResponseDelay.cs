using System;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Factories
{
    public interface IMockResponseDelay
    {
        /// <summary>
        /// Delays the response by the specified amount of time.
        /// </summary>
        /// <remarks>Used to make a Mock Response feel real, can also be used for testing how a system handles timeouts and or delays.</remarks>
        /// <param name="time">A <see cref="TimeSpan"/> representing how the long the Mock Response will be delayed by.</param>
        /// <param name="repeats">How many times this delay will be applied to the Mock Response. EG - If repeats is set to 2 and the request times out twice, the third attempt will not be delayed.</param>
        void IsDelayed(TimeSpan time, int repeats = 0);

        /// <summary>
        /// Delays the response by the specified amount of time.
        /// </summary>
        /// <remarks>Used to make a Mock Response feel real, can also be used for testing how a system handles timeouts and or delays.</remarks>
        /// <param name="seconds">How the long the Mock Response will be delayed by in seconds.</param>
        /// <param name="repeats">How many times this delay will be applied to the Mock Response. EG - If repeats is set to 2 and the request times out twice, the third attempt will not be delayed.</param>
        void IsDelayed(int seconds, int repeats = 0);
    }
}