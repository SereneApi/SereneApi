namespace SereneApi.Abstractions.Helpers
{
    public static class Defaults
    {
        public static uint MinimumRetryCount { get; set; } = 1;

        public static uint MaximumRetryCount { get; set; } = 5;
    }
}
