namespace SereneApi.Types.Dependencies
{
    public readonly struct RetryDependency
    {
        public uint Count { get; }

        public RetryDependency(uint count)
        {
            Count = count;
        }

        public static RetryDependency Default => new RetryDependency(ApiHandlerOptionDefaults.RetryCount);
    }
}
