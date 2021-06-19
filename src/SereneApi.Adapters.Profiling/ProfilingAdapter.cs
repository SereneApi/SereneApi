using SereneApi.Abstractions.Configuration.Adapters;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SereneApi.Adapters.Profiling
{
    /// <summary>
    /// After being initiated, will profile API usage once a session has been started. 
    /// </summary>
    public static class ProfilingAdapter
    {
        private static IProfiler _profiler;

        /// <summary>
        /// Specifies if a profiling session is currently in progress.
        /// </summary>
        private static bool HasActiveSession => _profiler.HasActiveSession;

        public static ISession Profile([NotNull] Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            StartSession();

            action.Invoke();

            return EndSession();
        }

        public static async Task<ISession> ProfileAsync([NotNull] Func<Task> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            StartSession();

            await action.Invoke();

            return EndSession();
        }

        /// <summary>
        /// Starts a profiling session.
        /// </summary>
        /// <remarks>Only one session can be run at a time.</remarks>
        /// <exception cref="MethodAccessException">Thrown when the testing adapter has not been initialized.</exception>
        /// <exception cref="MethodAccessException">Thrown when a session is already in progress.</exception>
        public static void StartSession()
        {
            if (_profiler == null)
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
            if (_profiler == null)
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
        public static IApiAdapter Initiate([NotNull] IApiAdapter adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            if (_profiler != null)
            {
                throw new MethodAccessException("Method cannot be called twice.");
            }

            if (adapter.Events == null)
            {
                throw new ArgumentNullException(nameof(adapter.Events));
            }

            _profiler = new Profiler(adapter.Events);

            return adapter;
        }

        /// <summary>
        /// Initiates a profiling adapter to all APIs.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a null value is provided.</exception>
        /// <exception cref="MethodAccessException">Thrown when this method is called more than once.</exception>
        public static IApiAdapter InitiateProfilingAdapter([NotNull] this IApiAdapter adapter)
        {
            return Initiate(adapter);
        }
    }
}
