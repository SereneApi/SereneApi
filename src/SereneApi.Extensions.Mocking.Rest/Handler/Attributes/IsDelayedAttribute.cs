using SereneApi.Extensions.Mocking.Rest.Configuration.Settings;
using System;

namespace SereneApi.Extensions.Mocking.Rest.Handler.Attributes
{
    /// <summary>
    /// Delays a response by the specified amount of time.
    /// <remarks>Can be applied to a Mock Method or the Mock Handler, if applied to both the Method attribute supersedes the Handler Attribute.</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IsDelayedAttribute : DelayedAttribute
    {
        public TimeSpan Delay { get; }

        public int Repeats { get; }

        /// <summary>
        /// Delays the response by the specified amount of time.
        /// </summary>
        /// <remarks>Used to make a Mock Response feel real, can also be used for testing how a system handles timeouts and or delays.</remarks>
        /// <param name="milliseconds">How the long the Mock Response will be delayed by in milliseconds.</param>
        /// <param name="repeats">How many times this delay will be applied to the Mock Response. EG - If repeats is set to 2 and the request times out twice, the third attempt will not be delayed.</param>
        public IsDelayedAttribute(int milliseconds = 1000, int repeats = 0)
        {
            Delay = TimeSpan.FromMilliseconds(milliseconds);
            Repeats = repeats;
        }

        public override IDelaySettings GetDelaySettings()
        {
            return new DelaySettings(Delay, Repeats);
        }
    }
}
