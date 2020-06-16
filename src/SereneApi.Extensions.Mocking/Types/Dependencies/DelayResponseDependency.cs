using System;
using System.Threading;
using System.Threading.Tasks;

namespace SereneApi.Extensions.Mocking.Types.Dependencies
{
    public class DelayResponseDependency
    {
        private int _delayCount;

        public TimeSpan DelayTime { get; }

        public int DelayCount { get; }

        public DelayResponseDependency(int seconds, int delayCount = 0)
        {
            DelayTime = new TimeSpan(0, 0, seconds);
            DelayCount = delayCount;
            _delayCount = delayCount;
        }

        public Task DelayAsync(CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                if (DelayCount > 0)
                {
                    if (_delayCount <= 0)
                    {
                        return;
                    }

                    _delayCount--;
                }

                bool canceled = cancellationToken.WaitHandle.WaitOne(DelayTime);

                if (canceled)
                {
                    throw new TaskCanceledException("The response was canceled as it breached the timeout time.");
                }
            }, cancellationToken);
        }
    }
}
