namespace DeltaWare.SereneApi.Types.Dependencies
{
    public readonly struct RetryDependency
    {
        public static RetryDependency Default => new RetryDependency(ApiHandlerOptionDefaults.RetryCount);

        public uint Count { get; }

        public RetryDependency(uint count)
        {
            Count = count;
        }
    }
}
