using System;

namespace SereneApi.Helpers
{
    public class ApiHandlerOptionsRules
    {
        public const uint MinimumRetryCount = 1;

        public const uint MaximumRetryCount = 5;


        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the Retry Count is invalid.
        /// </summary>
        public static void ValidateRetryAttempts(int retryCount)
        {
            if(retryCount < MinimumRetryCount)
            {
                throw new ArgumentException($"To Enable Retry on Timeout the RetryCount must be greater than or equal to {MinimumRetryCount}");
            }

            if(retryCount > MaximumRetryCount)
            {
                throw new ArgumentException($"To Enable Retry on Timeout the RetryCount must be less than or equal to {MaximumRetryCount}");
            }
        }
    }
}
