using System;

namespace SereneApi.Abstractions
{
    public interface IConnectionSettings
    {
        Uri BaseAddress { get; }

        public string Source { get; }

        string Resource { get; }

        /// <summary>
        /// The Path to the resource.
        /// </summary>
        string ResourcePath { get; }

        int Timeout { get; }

        int RetryAttempts { get; }
    }
}
