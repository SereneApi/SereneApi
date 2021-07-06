using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace SereneApi
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
