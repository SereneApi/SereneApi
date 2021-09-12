using System;

namespace SereneApi.Core.Helpers
{
    public static class Rules
    {
        public static uint MaximumRetryCount { get; } = 5;
        public static uint MinimumRetryCount { get; } = 1;

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the Retry Count is invalid.
        /// </summary>
        public static void ValidateRetryAttempts(int retryCount)
        {
            if (retryCount < MinimumRetryCount)
            {
                throw new ArgumentException($"To Enable Retry on Timeout the RetryCount must be greater than or equal to {MinimumRetryCount}");
            }

            if (retryCount > MaximumRetryCount)
            {
                throw new ArgumentException($"To Enable Retry on Timeout the RetryCount must be less than or equal to {MaximumRetryCount}");
            }
        }
    }
}