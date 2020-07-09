using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Dependencies
{
    /// <summary>
    /// Delays the response by the specified amount of time.
    /// </summary>
    public class DelayedResponseDependency
    {
        private int _timesDelayed;

        /// <summary>
        /// The specified amount of time the response will be delayed by.
        /// </summary>
        public TimeSpan DelayTime { get; }

        /// <summary>
        /// The amount of times this response will be delayed.
        /// </summary>
        public int DelayAmount { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="DelayedResponseDependency"/>.
        /// </summary>
        /// <param name="seconds">The number of seconds the response will be delayed for.</param>
        /// <param name="delayAmount">How many times the response will be delayed.</param>
        /// <remarks>If the specified delayed amount is 0 every response will be delayed.</remarks>
        public DelayedResponseDependency(int seconds, int delayAmount = 0)
        {
            DelayTime = TimeSpan.FromSeconds(seconds);
            DelayAmount = delayAmount;
            _timesDelayed = delayAmount;
        }

        /// <summary>
        /// Delays the response asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancels the delayed response.</param>
        /// <exception cref="TaskCanceledException">Thrown when a <see cref="CancellationToken"/> is received.</exception>
        public Task DelayResponseAsync(CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                if(DelayAmount > 0)
                {
                    if(_timesDelayed <= 0)
                    {
                        return;
                    }

                    _timesDelayed--;
                }

                bool canceled = cancellationToken.WaitHandle.WaitOne(DelayTime);

                if(canceled)
                {
                    throw new TaskCanceledException("The response was canceled as it breached the timeout time.");
                }
            }, cancellationToken);
        }
    }
}
