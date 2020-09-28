using System;

namespace SereneApi.Adapters.Testing.Profiling
{
    public interface IProfiler
    {
        /// <summary>
        /// Specifies if a profiling session is currently in progress.
        /// </summary>
        public bool HasActiveSession { get; }

        /// <summary>
        /// Starts a profiling session.
        /// </summary>
        /// <remarks>Only one session can be run at a time.</remarks>
        void StartSession();

        /// <summary>
        /// Ends the profiling session returning the session statistics.
        /// </summary>
        /// <exception cref="MethodAccessException">Thrown if a session was not already started.</exception>
        ISession EndSession();
    }
}
