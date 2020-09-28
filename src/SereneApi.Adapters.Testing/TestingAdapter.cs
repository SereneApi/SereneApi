using SereneApi.Abstractions.Configuration;
using SereneApi.Adapters.Testing.Profiling;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Adapters.Testing
{
    public static class TestingAdapter
    {
        private static IProfiler _profiler;

        /// <summary>
        /// Specifies if a profiling session is currently in progress.
        /// </summary>
        private static bool HasActiveSession => _profiler.IsActive;

        /// <summary>
        /// Starts a profiling session.
        /// </summary>
        /// <remarks>Only one session can be run at a time.</remarks>
        public static void StartSession()
        {
            _profiler.StartSession();
        }

        /// <summary>
        /// Ends the profiling session returning the session statistics.
        /// </summary>
        /// <exception cref="MethodAccessException">Thrown if a session was not already started.</exception>
        public static ISession EndSession()
        {
            return _profiler.EndSession();
        }

        /// <summary>
        /// Adds a profiling adapter to all APIs.
        /// </summary>
        /// <param name="extensions"></param>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when this method is called more than once.</exception>
        public static IApiConfigurationExtensions AddProfilingAdapter([NotNull] this IApiConfigurationExtensions extensions)
        {
            if(extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }

            if(_profiler != null)
            {
                throw new MethodAccessException("Method cannot be called twice.");
            }

            if(extensions.EventRelay == null)
            {
                extensions.EnableEvents();
            }

            _profiler = new Profiler(extensions.EventRelay);

            return extensions;
        }
    }
}
