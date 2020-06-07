using SereneApi.Helpers;

namespace SereneApi.Types.Dependencies
{
    public readonly struct RetryDependency
    {
        public int Count { get; }

        public RetryDependency(int count)
        {
            Count = count;
        }

        public static RetryDependency Default => new RetryDependency(ApiHandlerOptionDefaults.RetryCount);
    }
}
