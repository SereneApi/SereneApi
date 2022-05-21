using SereneApi.Extensions.Mocking.Rest.Configuration.Settings;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Delays a response by the specified amount of time.
    /// <remarks>Can be applied to a Mock Method or the Mock Handler, if applied to both the Method attribute supersedes the Handler Attribute.</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IsDelayedRandomlyAttribute : DelayedAttribute
    {
        public int MinMilliseconds { get; }

        public int MaxMilliseconds { get; }

        public int Repeats { get; }

        private readonly Random _random = new(DateTime.Now.Millisecond);

        /// <summary>
        /// Delays the response by a random amount of time.
        /// </summary>
        /// <remarks>Used to make a Mock Response feel real, can also be used for testing how a system handles timeouts and or delays.</remarks>
        /// <param name="minMilliseconds">The minimum amount of milliseconds the response can be delayed by.</param>
        /// <param name="maxMilliseconds">The maximum amount of milliseconds the response can be delayed by.</param>
        /// <param name="repeats">How many times this delay will be applied to the Mock Response. EG - If repeats is set to 2 and the request times out twice, the third attempt will not be delayed.</param>
        public IsDelayedRandomlyAttribute(int minMilliseconds = 100, int maxMilliseconds = 1000, int repeats = 0)
        {
            MinMilliseconds = minMilliseconds;
            MaxMilliseconds = maxMilliseconds;
            Repeats = repeats;
        }

        public override IDelaySettings GetDelaySettings()
        {
            int milliseconds = _random.Next(MinMilliseconds, MaxMilliseconds);

            return new DelaySettings(TimeSpan.FromMilliseconds(milliseconds), Repeats);
        }
    }
}
