using System;

namespace SereneApi.Extensions.Mocking.Rest.Configuration.Settings
{
    /// <summary>
    /// Specifies how a response will be delayed.
    /// </summary>
    public interface IDelaySettings
    {
        /// <summary>
        /// Specifies how many times a response will be delayed. After the delay has been repeated by the specified amount of times it will no longer be delayed.
        /// </summary>
        /// <remarks>A value of 0 specified that the delay will never end.</remarks>
        int Repeats { get; }

        /// <summary>
        /// Specifies the amount of time to delay the response.
        /// </summary>
        /// <remarks>Execution time is not taken into account.</remarks>
        TimeSpan Time { get; }
    }
}
