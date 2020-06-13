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

        public Task DelayAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                if (_delayCount <= 0)
                {
                    return;
                }

                _delayCount--;

                Thread.Sleep(DelayTime);
            });
        }
    }
}
