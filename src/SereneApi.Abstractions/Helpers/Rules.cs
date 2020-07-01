using System;

namespace SereneApi.Abstractions.Helpers
{
    public static class Rules
    {



        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the Retry Count is invalid.
        /// </summary>
        public static void ValidateRetryAttempts(int retryCount)
        {
            if(retryCount < Defaults.MinimumRetryCount)
            {
                throw new ArgumentException($"To Enable Retry on Timeout the RetryCount must be greater than or equal to {Defaults.MinimumRetryCount}");
            }

            if(retryCount > Defaults.MaximumRetryCount)
            {
                throw new ArgumentException($"To Enable Retry on Timeout the RetryCount must be less than or equal to {Defaults.MaximumRetryCount}");
            }
        }
    }
}
