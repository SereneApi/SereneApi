namespace SereneApi.Core
{
    public static class Logging
    {
        public static class EventIds
        {
            public const int AuthorizationEvent = 1050;
            public const int DependencyNotFound = 4000;
            public const int DisposedEvent = 1001;
            public const int ExceptionEvent = 5000;
            public const int InstantiatedEvent = 1000;
            public const int InvalidMethodForRequestEvent = 4001;
            public const int PerformRequestEvent = 2000;
            public const int ResponseReceivedEvent = 2001;

            public const int RetryEvent = 3000;
        }

        public static class Messages
        {
            /// <summary>
            /// ERROR - Params (Handler Type)
            /// </summary>
            public const string AccessOfDisposedHandler = "[{ApiHandler}] was accessed after being disposed";

            /// <summary>
            /// DEBUG
            /// </summary>
            public const string AuthorizationTokenCached = "Using Cached authorization token";

            /// <summary>
            /// DEBUG
            /// </summary>
            public const string AuthorizationTokenRenewal = "Renewing Cached authorization token";

            /// <summary>
            /// ERROR - Params(Type)
            /// </summary>
            public const string DependencyNotFound = "Could not retrieve the dependency for [{DependencyType}]";

            /// <summary>
            /// DEBUG - Params(HttpMethod, Route)
            /// </summary>
            public const string DisposedHandler = "[{ApiHandler}] has been disposed";

            /// <summary>
            /// DEBUG - Params(HttpMethod, Route)
            /// </summary>
            public const string DisposedHttpClient = "The HttpClient used for the [{HttpMethod}] request to \"{Route}\" has been disposed";

            /// <summary>
            /// DEBUG - Params(HttpMethod, Route)
            /// </summary>
            public const string DisposedHttpResponseMessage = "The HttpResponseMessage received by the [{HttpMethod}] request to \"{Route}\" has been disposed";

            /// <summary>
            /// TRACE - Params (Handler Type)
            /// </summary>
            public const string HandlerInstantiated = "[{ApiHandler}] Instantiated";

            /// <summary>
            /// ERROR - Params (HttpMethod)
            /// </summary>
            public const string InvalidMethodForInBodyContent = "[{HttpMethod}] is an invalid httpMethod for a request with in body content";

            /// <summary>
            /// DEBUG - Params (HttpMethod, Route)
            /// </summary>
            public const string PerformingRequest = "Performing a [{HttpMethod}] request against \"{Route}\"";

            /// <summary>
            /// DEBUG - Params (HttpMethod, Route)
            /// </summary>
            public const string PerformingRequestWithContent = "Performing a [{HttpMethod}] request against \"{Route}\" with content [{HttpContent}]";

            /// <summary>
            /// INFORMATION - Params (HttpMethod, Route, Status)
            /// </summary>
            public const string ReceivedResponse = "Received response from [{HttpMethod}] request to \"{Route}\" with a status of [{HttpStatus}]";

            /// <summary>
            /// ERROR - Params(HttpMethod, Route)
            /// </summary>
            public const string RequestEncounteredException = "An exception was encountered when performing a [{HttpMethod}] request to \"{Route}\"";

            /// <summary>
            /// ERROR - Params(HttpMethod, Route, Attempts)
            /// </summary>
            public const string TimeoutNoRetry = "The [{HttpMethod}] request to \"{Route}\" has Timed out; The retry limit has been reached after attempting {Count} times";

            /// <summary>
            /// ERROR - Params(HttpMethod, Route)
            /// </summary>
            public const string Timeout = "The [{HttpMethod}] request to \"{Route}\" has Timed out";

            /// <summary>
            /// WARNING - Params (HttpMethod, Route, Remaining Attempts)
            /// </summary>
            public const string TimeoutRetry = "The [{HttpMethod}] request to \"{Route}\" has Timed out, retrying request. {Count} attempts remaining";
        }
    }
}