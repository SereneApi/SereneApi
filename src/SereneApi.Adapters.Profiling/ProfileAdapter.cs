using SereneApi.Abstractions.Configuration;
using SereneApi.Adapters.Profiling.Profiling;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Adapters.Profiling
{
    public static class ProfileAdapter
    {
        private static IProfiler _profiler;

        /// <summary>
        /// Specifies if a profiling session is currently in progress.
        /// </summary>
        private static bool HasActiveSession => _profiler.HasActiveSession;

        /// <summary>
        /// Starts a profiling session.
        /// </summary>
        /// <remarks>Only one session can be run at a time.</remarks>
        /// <exception cref="MethodAccessException">Thrown when the testing adapter has not been initialized.</exception>
        /// <exception cref="MethodAccessException">Thrown when a session is already in progress.</exception>
        public static void StartSession()
        {
            if(_profiler == null)
            {
                throw new MethodAccessException("TestingAdapter must be initiate first.");
            }

            _profiler.StartSession();
        }

        /// <summary>
        /// Ends the profiling session returning the session statistics.
        /// </summary>
        /// <exception cref="MethodAccessException">Thrown when the testing adapter has not been initialized.</exception>
        /// <exception cref="MethodAccessException">Thrown when a session was not in progress.</exception>
        public static ISession EndSession()
        {
            if(_profiler == null)
            {
                throw new MethodAccessException("TestingAdapter must be initiate first.");
            }

            return _profiler.EndSession();
        }

        /// <summary>
        /// Initiates a profiling adapter to all APIs.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when this method is called more than once.</exception>
        public static IApiAdapter InitiateProfilingAdapter([NotNull] this IApiAdapter adapter)
        {
            if(adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            if(_profiler != null)
            {
                throw new MethodAccessException("Method cannot be called twice.");
            }

            if(adapter.EventRelay == null)
            {   
                throw new ArgumentNullException(nameof(adapter.EventRelay));
            }

            _profiler = new Profiler(adapter.EventRelay);

            return adapter;
        }
    }
}
