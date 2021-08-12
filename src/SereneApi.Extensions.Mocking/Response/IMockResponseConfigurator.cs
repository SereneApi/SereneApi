using System;

namespace SereneApi.Extensions.Mocking.Response
{
    public interface IMockResponseConfigurator : IMockResponseMethod
    {
        /// <summary>
        /// Delays the <see cref="IMockResponse"/> by the specified number of seconds.
        /// </summary>
        /// <remarks>This can be useful for testing timeouts and latency.</remarks>
        /// <param name="seconds">The number of seconds the response will be delayed by.</param>
        /// <param name="delayCount">How many times this request will be delayed before.</param>
        /// <exception cref="ArgumentException">Thrown if an invalid value is provided.</exception>
        IMockResponseMethod ResponseIsDelayed(int seconds, int delayCount = 0);
    }
}
