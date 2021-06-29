namespace SereneApi
{
    internal static class Logging
    {
        internal static class Messages
        {
            /// <summary>
            /// TRACE - Params (Handler Type)
            /// </summary>
            public const string HandlerInstantiated = "[{ApiHandler}] Instantiated";

            /// <summary>
            /// ERROR - Params (Method)
            /// </summary>
            public const string InvalidMethodForInBodyContent = "[{HttpMethod}] is an invalid method for a request with in body content";

            /// <summary>
            /// DEBUG - Params (Method, Route)
            /// </summary>
            public const string PerformingRequest = "Performing a [{HttpMethod}] request against \"{Route}\"";

            /// <summary>
            /// DEBUG - Params (Method, Route)
            /// </summary>
            public const string PerformingRequestWithContent = "Performing a [{HttpMethod}] request against \"{Route}\" with content [{HttpContent}]";

            /// <summary>
            /// INFORMATION - Params (Method, Route, Status)
            /// </summary>
            public const string ReceivedResponse = "Received response from [{HttpMethod}] request to \"{Route}\" with a status of [{HttpStatus}]";

            /// <summary>
            /// WARNING - Params (Method, Route, Remaining Attempts)
            /// </summary>
            public const string TimeoutRetry = "The [{HttpMethod}] request to \"{Route}\" has Timed out, retrying request. {Count} attempts remaining";

            /// <summary>
            /// ERROR - Params(Method, Route, Attempts)
            /// </summary>
            public const string TimeoutNoRetry = "The [{HttpMethod}] request to \"{Route}\" has Timed out; The retry limit has been reached after attempting {Count} times";

            /// <summary>
            /// DEBUG - Params(Method, Route)
            /// </summary>
            public const string DisposedHttpClient = "The HttpClient used for the [{HttpMethod}] request to \"{Route}\" has been disposed";

            /// <summary>
            /// DEBUG - Params(Method, Route)
            /// </summary>
            public const string DisposedHttpResponseMessage = "The HttpResponseMessage received by the [{HttpMethod}] request to \"{Route}\" has been disposed";

            /// <summary>
            /// DEBUG - Params (Handler Type)
            /// </summary>
            public const string DisposedHandler = "[{ApiHandler}] has been disposed";

            /// <summary>
            /// ERROR - Params(Method, Route)
            /// </summary>
            public const string RequestEncounteredException = "An exception was encountered when performing a [{HttpMethod}] request to \"{Route}\"";
        }

        internal static class EventIds
        {

        }
    }
}
