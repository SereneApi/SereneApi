using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Core.Extensions
{
    public static class TaskExtensions
    {
        public static async void FireAndForget(this Task task, [AllowNull] Action<Exception> onException = null)
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                onException?.Invoke(exception);
            }
        }
    }
}
