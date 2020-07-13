
using System;
using System.Diagnostics.CodeAnalysis;

namespace SereneApi.Abstractions.Response
{
    public static class ApiResponseExtensions
    {
        /// <summary>
        /// Specifies if the response was not successful.
        /// </summary>
        public static bool WasNotSuccessful([NotNull] this IApiResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return !response.WasSuccessful;
        }

        /// <summary>
        /// Specifies if the response was not successful.
        /// </summary>
        public static bool HasNullResult<TResponse>([NotNull] this IApiResponse<TResponse> response)
        {
            if(response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            return response.Result == null;
        }
    }
}
